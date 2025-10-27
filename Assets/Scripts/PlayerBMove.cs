using UnityEngine;

public class PlayerBMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float acceleration = 10f;
    public float jumpForce = 14f;
    public int maxJumps = 2;

    [Header("Physics Settings")]
    public float airControlMultiplier = 0.5f;
    public float slopeFriction = 0.9f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private int jumpCount;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Debug.Log("PlayerBMove initialized!");
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (isGrounded)
            jumpCount = 0;

        // Jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            jumpCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            Debug.Log($"Jump {jumpCount}/{maxJumps}");
        }

        // Flip sprite
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        // Animation
        if (anim)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * moveSpeed;
        float control = isGrounded ? 1f : airControlMultiplier;
        float newSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, control * acceleration * Time.fixedDeltaTime);

        // Apply slope friction if nearly stopped
        if (isGrounded && Mathf.Abs(moveInput) < 0.1f)
            newSpeed *= slopeFriction;

        rb.velocity = new Vector2(newSpeed, rb.velocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            Debug.Log("PlayerBMove hit obstacle!");
    }
}
