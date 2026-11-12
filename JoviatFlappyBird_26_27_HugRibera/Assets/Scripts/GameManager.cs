using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private enum GameState
    {
        Ready,
        Playing,
        GameOver
    }

    private Player player;
    private PipeSpawner pipeSpawner;
    private TMP_Text statusText;
    private AudioSource audioSource;
    private GameObject newRecordDialog;
    private TMP_Text newRecordDialogText;
    private GameObject resetConfirmationDialog;
    private GameObject resetBestScoreButton;
    private GameState state = GameState.Ready;
    private int score;
    private int bestScore;

    private const string BestScoreKey = "JoviatFlappyBird_BestScore";

    [SerializeField] private AudioClip wingClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip pointClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip swooshingClip;

    public bool IsPlaying => state == GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Only one GameManager can be active at a time.", this);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        pipeSpawner = FindAnyObjectByType<PipeSpawner>();
        statusText = FindAnyObjectByType<TMP_Text>();
        bestScore = Mathf.Max(0, PlayerPrefs.GetInt(BestScoreKey, 0));

        if (player == null)
        {
            Debug.LogError("GameManager could not find a Player in the scene.", this);
        }

        if (pipeSpawner == null)
        {
            Debug.LogError("GameManager could not find a PipeSpawner in the scene.", this);
        }

        if (statusText == null)
        {
            Debug.LogError("GameManager could not find a TextMeshPro text for the score.", this);
        }
        else
        {
            CreateBestScoreResetButton();
            CreateNewRecordDialog();
            CreateResetConfirmationDialog();
        }

        UpdateStatusText();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool HandleFlapInput()
    {
        if (state == GameState.Ready)
        {
            StartGame();
        }
        else if (state == GameState.GameOver)
        {
            RestartGame();
        }

        return IsPlaying;
    }

    public void AddScore()
    {
        if (!IsPlaying)
        {
            return;
        }

        score++;
        PlaySound(pointClip);
        UpdateStatusText();
    }

    public void GameOver()
    {
        if (!IsPlaying)
        {
            return;
        }

        state = GameState.GameOver;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
            ShowNewRecordDialog();
            PlaySound(pointClip);
        }

        PlaySound(dieClip);
        UpdateStatusText();
    }

    public void PlayFlapSound()
    {
        PlaySound(wingClip);
    }

    public void PlayHitSound()
    {
        PlaySound(hitClip);
    }

    private void StartGame()
    {
        state = GameState.Playing;
        PlaySound(swooshingClip);
        UpdateStatusText();
    }

    private void RestartGame()
    {
        score = 0;
        if (newRecordDialog != null)
        {
            newRecordDialog.SetActive(false);
        }

        player.ResetBird();
        pipeSpawner.ResetPipes();
        StartGame();
    }

    private void CreateNewRecordDialog()
    {
        Canvas canvas = statusText.canvas;
        if (canvas == null)
        {
            Debug.LogError("GameManager could not find a Canvas for the new record dialog.", this);
            return;
        }

        newRecordDialog = new GameObject("New Record Dialog", typeof(RectTransform), typeof(Image));
        newRecordDialog.transform.SetParent(canvas.transform, false);

        RectTransform dialogRect = newRecordDialog.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.sizeDelta = new Vector2(440f, 200f);

        Image background = newRecordDialog.GetComponent<Image>();
        background.color = new Color(0.12f, 0.1f, 0.08f, 0.95f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(newRecordDialog.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 16f);
        textRect.offsetMax = new Vector2(-16f, -16f);

        newRecordDialogText = textObject.GetComponent<TextMeshProUGUI>();
        newRecordDialogText.font = statusText.font;
        newRecordDialogText.fontSize = 36f;
        newRecordDialogText.color = Color.white;
        newRecordDialogText.alignment = TextAlignmentOptions.Center;
        newRecordDialogText.raycastTarget = false;
        newRecordDialog.SetActive(false);
    }

    private void CreateBestScoreResetButton()
    {
        Canvas canvas = statusText.canvas;
        if (canvas == null)
        {
            Debug.LogError("GameManager could not find a Canvas for the reset button.", this);
            return;
        }

        resetBestScoreButton = CreateButton(
            canvas.transform,
            "Reset Best Score",
            "RESET BEST",
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-95f, -36f),
            new Vector2(170f, 52f),
            new Color(0.55f, 0.18f, 0.12f, 1f),
            ShowResetConfirmation);
    }

    private void CreateResetConfirmationDialog()
    {
        Canvas canvas = statusText.canvas;
        if (canvas == null)
        {
            Debug.LogError("GameManager could not find a Canvas for reset confirmation.", this);
            return;
        }

        resetConfirmationDialog = new GameObject("Reset Best Score Confirmation", typeof(RectTransform), typeof(Image));
        resetConfirmationDialog.transform.SetParent(canvas.transform, false);

        RectTransform overlayRect = resetConfirmationDialog.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlay = resetConfirmationDialog.GetComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject panel = new GameObject("Confirmation Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(resetConfirmationDialog.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(460f, 230f);

        Image panelBackground = panel.GetComponent<Image>();
        panelBackground.color = new Color(0.12f, 0.1f, 0.08f, 1f);

        GameObject messageObject = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
        messageObject.transform.SetParent(panel.transform, false);

        RectTransform messageRect = messageObject.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0f, 0.35f);
        messageRect.anchorMax = Vector2.one;
        messageRect.offsetMin = new Vector2(16f, 0f);
        messageRect.offsetMax = new Vector2(-16f, -12f);

        TMP_Text message = messageObject.GetComponent<TMP_Text>();
        message.font = statusText.font;
        message.text = "Reset the Best Score?\nThis cannot be undone.";
        message.fontSize = 26f;
        message.color = Color.white;
        message.alignment = TextAlignmentOptions.Center;
        message.raycastTarget = false;

        CreateButton(
            panel.transform,
            "Cancel Reset",
            "CANCEL",
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            new Vector2(125f, 50f),
            new Vector2(180f, 52f),
            new Color(0.3f, 0.3f, 0.3f, 1f),
            CancelResetConfirmation);

        CreateButton(
            panel.transform,
            "Confirm Reset",
            "RESET",
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-125f, 50f),
            new Vector2(180f, 52f),
            new Color(0.65f, 0.16f, 0.12f, 1f),
            ResetBestScore);

        resetConfirmationDialog.SetActive(false);
    }

    private GameObject CreateButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 size,
        Color color,
        UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.anchoredPosition = anchoredPosition;
        buttonRect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.color = color;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(6f, 4f);
        labelRect.offsetMax = new Vector2(-6f, -4f);

        TMP_Text buttonText = labelObject.GetComponent<TMP_Text>();
        buttonText.font = statusText.font;
        buttonText.text = label;
        buttonText.fontSize = 20f;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.raycastTarget = false;

        return buttonObject;
    }

    private void ShowResetConfirmation()
    {
        if (resetConfirmationDialog == null)
        {
            Debug.LogError("GameManager could not show reset confirmation.", this);
            return;
        }

        resetConfirmationDialog.SetActive(true);
    }

    private void CancelResetConfirmation()
    {
        if (resetConfirmationDialog != null)
        {
            resetConfirmationDialog.SetActive(false);
        }
    }

    private void ResetBestScore()
    {
        PlayerPrefs.DeleteKey(BestScoreKey);
        PlayerPrefs.Save();
        bestScore = 0;

        if (newRecordDialog != null)
        {
            newRecordDialog.SetActive(false);
        }

        CancelResetConfirmation();
        UpdateStatusText();
    }

    private void ShowNewRecordDialog()
    {
        if (newRecordDialog == null || newRecordDialogText == null)
        {
            Debug.LogError("GameManager could not show the new record dialog.", this);
            return;
        }

        newRecordDialogText.text = $"NEW RECORD!\nScore: {score}";
        newRecordDialog.SetActive(true);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("GameManager is missing an audio clip assigned in the Inspector.", this);
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    private void UpdateStatusText()
    {
        if (statusText == null)
        {
            return;
        }

        switch (state)
        {
            case GameState.Ready:
                statusText.text = $"Current Score: {score}\nBest Score: {bestScore}\nSPACE / CLICK TO START";
                break;
            case GameState.Playing:
                statusText.text = $"Current Score: {score}\nBest Score: {bestScore}";
                break;
            case GameState.GameOver:
                statusText.text = $"GAME OVER\nCurrent Score: {score}\nBest Score: {bestScore}\nSPACE / CLICK TO RESTART";
                break;
        }

        if (resetBestScoreButton != null)
        {
            resetBestScoreButton.SetActive(state != GameState.Playing);
        }
    }
}
