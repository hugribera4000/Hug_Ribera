using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;

    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer secondBackgroundRenderer;
    private Camera gameCamera;
    private float backgroundWidth;

    private void Awake()
    {
        backgroundRenderer = GetComponent<SpriteRenderer>();
        if (backgroundRenderer == null)
        {
            Debug.LogError("BackgroundScroller needs a SpriteRenderer.", this);
        }
    }

    private void Start()
    {
        gameCamera = Camera.main;
        if (gameCamera == null || !gameCamera.orthographic)
        {
            Debug.LogError("BackgroundScroller needs an orthographic Main Camera.", this);
            return;
        }

        if (backgroundRenderer == null)
        {
            return;
        }

        backgroundWidth = backgroundRenderer.bounds.size.x;
        if (backgroundWidth <= 0f)
        {
            Debug.LogError("BackgroundScroller needs a background with non-zero width.", this);
            return;
        }

        GameObject copy = new GameObject($"{gameObject.name} (Scrolling Copy)");
        copy.transform.position = transform.position + Vector3.right * backgroundWidth;
        copy.transform.rotation = transform.rotation;
        copy.transform.localScale = transform.lossyScale;
        copy.transform.SetParent(transform.parent, true);

        secondBackgroundRenderer = copy.AddComponent<SpriteRenderer>();
        secondBackgroundRenderer.sprite = backgroundRenderer.sprite;
        secondBackgroundRenderer.sharedMaterial = backgroundRenderer.sharedMaterial;
        secondBackgroundRenderer.color = backgroundRenderer.color;
        secondBackgroundRenderer.flipX = backgroundRenderer.flipX;
        secondBackgroundRenderer.flipY = backgroundRenderer.flipY;
        secondBackgroundRenderer.drawMode = backgroundRenderer.drawMode;
        secondBackgroundRenderer.size = backgroundRenderer.size;
        secondBackgroundRenderer.sortingLayerID = backgroundRenderer.sortingLayerID;
        secondBackgroundRenderer.sortingOrder = backgroundRenderer.sortingOrder;
        secondBackgroundRenderer.maskInteraction = backgroundRenderer.maskInteraction;
    }

    private void Update()
    {
        if (secondBackgroundRenderer == null ||
            gameCamera == null ||
            GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
        {
            return;
        }

        Vector3 movement = Vector3.left * scrollSpeed * Time.deltaTime;
        backgroundRenderer.transform.position += movement;
        secondBackgroundRenderer.transform.position += movement;

        float cameraLeftEdge = gameCamera.transform.position.x - gameCamera.orthographicSize * gameCamera.aspect;
        MoveOffscreenBackgroundToEnd(backgroundRenderer, secondBackgroundRenderer, cameraLeftEdge);
        MoveOffscreenBackgroundToEnd(secondBackgroundRenderer, backgroundRenderer, cameraLeftEdge);
    }

    private void MoveOffscreenBackgroundToEnd(
        SpriteRenderer background,
        SpriteRenderer otherBackground,
        float cameraLeftEdge)
    {
        if (background.bounds.max.x <= cameraLeftEdge)
        {
            float halfWidth = backgroundWidth * 0.5f;
            Vector3 position = background.transform.position;
            position.x = otherBackground.bounds.max.x + halfWidth;
            background.transform.position = position;
        }
    }
}
