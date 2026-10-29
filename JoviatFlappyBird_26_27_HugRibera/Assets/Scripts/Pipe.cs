using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float destroyX = -12f;

    private Player player;
    private Rigidbody2D rb;
    private bool hasScored;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Pipe could not find a Player to score against.", this);
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Pipe needs a Rigidbody2D component.", this);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null || GameManager.Instance == null || !GameManager.Instance.IsPlaying)
        {
            return;
        }

        Vector2 nextPosition = rb.position + Vector2.left * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);

        if (!hasScored && player != null && nextPosition.x < player.transform.position.x)
        {
            hasScored = true;
            GameManager.Instance.AddScore();
        }

        if (nextPosition.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
