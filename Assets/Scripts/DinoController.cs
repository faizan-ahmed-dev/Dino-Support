using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DinoController : MonoBehaviour
{
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    public bool inputEnabled = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!inputEnabled) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                         || Input.GetKeyDown(KeyCode.UpArrow)
                         || Input.GetMouseButtonDown(0);

        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void ResetDino(Vector3 startPosition)
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        inputEnabled = true;
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