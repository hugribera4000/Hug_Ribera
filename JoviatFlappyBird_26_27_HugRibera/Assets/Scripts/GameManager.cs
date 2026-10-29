using UnityEngine;
using TMPro;

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
    private GameState state = GameState.Ready;
    private int score;

    public bool IsPlaying => state == GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Only one GameManager can be active at a time.", this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        pipeSpawner = FindAnyObjectByType<PipeSpawner>();
        statusText = FindAnyObjectByType<TMP_Text>();

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
        UpdateStatusText();
    }

    public void GameOver()
    {
        if (!IsPlaying)
        {
            return;
        }

        state = GameState.GameOver;
        UpdateStatusText();
    }

    private void StartGame()
    {
        state = GameState.Playing;
        UpdateStatusText();
    }

    private void RestartGame()
    {
        score = 0;
        player.ResetBird();
        pipeSpawner.ResetPipes();
        StartGame();
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
                statusText.text = "Score: 0\nSPACE / CLICK TO START";
                break;
            case GameState.Playing:
                statusText.text = $"Score: {score}";
                break;
            case GameState.GameOver:
                statusText.text = $"GAME OVER\nScore: {score}\nSPACE / CLICK TO RESTART";
                break;
        }
    }
}
