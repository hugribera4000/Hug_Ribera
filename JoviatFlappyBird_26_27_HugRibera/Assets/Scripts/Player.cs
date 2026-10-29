using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float flapForce = 5f;
    [SerializeField] private float gravityScale = 1.5f;
    [SerializeField] private Sprite[] flapFrames;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingPosition;
    private Sprite startingSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPosition = transform.position;

        if (rb == null)
        {
            Debug.LogError("Player needs a Rigidbody2D component.", this);
        }

        if (spriteRenderer != null)
        {
            startingSprite = spriteRenderer.sprite;
        }
    }

    private void Start()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying &&
            spriteRenderer != null && flapFrames != null && flapFrames.Length > 0)
        {
            int frame = Mathf.FloorToInt(Time.time * 12f) % flapFrames.Length;
            spriteRenderer.sprite = flapFrames[frame];
        }

        if (!WasFlapPressed())
        {
            return;
        }

        GameManager gameManager = GameManager.Instance;
        if (rb == null || gameManager == null || !gameManager.HandleFlapInput())
        {
            return;
        }

        rb.gravityScale = gravityScale;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (rb == null || GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        float targetAngle = Mathf.Clamp(rb.linearVelocity.y * 6f, -70f, 35f);
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, targetAngle, Time.fixedDeltaTime * 5f));

        if (transform.position.y < -6f || transform.position.y > 6f)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ResetBird()
    {
        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = startingSprite;
        }
    }

    private static bool WasFlapPressed()
    {
        return (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)) ||
               (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
    }
}
