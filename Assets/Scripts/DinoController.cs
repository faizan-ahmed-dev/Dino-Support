using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class DinoController : MonoBehaviour
{
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Duck Sprite (optional - assign once you have real dino art)")]
    public SpriteRenderer dinoRenderer;
    public Sprite standingSprite;
    public Sprite duckingSprite;

    [Header("Duck (collider shrinks instantly - no animation, just a state swap)")]
    public Vector2 standingColliderSize = new Vector2(1f, 1f);
    public Vector2 standingColliderOffset = Vector2.zero;
    public Vector2 duckingColliderSize = new Vector2(1f, 0.5f);
    public Vector2 duckingColliderOffset = new Vector2(0f, -0.25f);

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isGrounded;
    private bool isDucking;
    public bool inputEnabled = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        // Record whatever your collider's current size/offset already is as the
        // "standing" state, so this matches your existing setup automatically.
        standingColliderSize = col.size;
        standingColliderOffset = col.offset;
    }

    void Update()
    {
        if (!inputEnabled) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                         || Input.GetKeyDown(KeyCode.UpArrow)
                         || Input.GetMouseButtonDown(0);

        if (jumpPressed && isGrounded && !isDucking)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        bool duckHeld = Input.GetKey(KeyCode.DownArrow);
        if (duckHeld && isGrounded && !isDucking)
        {
            isDucking = true;
            col.size = duckingColliderSize;
            col.offset = duckingColliderOffset;
            if (dinoRenderer != null && duckingSprite != null) dinoRenderer.sprite = duckingSprite;
        }
        else if ((!duckHeld || !isGrounded) && isDucking)
        {
            isDucking = false;
            col.size = standingColliderSize;
            col.offset = standingColliderOffset;
            if (dinoRenderer != null && standingSprite != null) dinoRenderer.sprite = standingSprite;
        }
    }

    public void ResetDino(Vector3 startPosition)
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        inputEnabled = true;
        isDucking = false;
        col.size = standingColliderSize;
        col.offset = standingColliderOffset;
    }

    public void FreezeDino()
    {
        inputEnabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void UnfreezeDino()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}