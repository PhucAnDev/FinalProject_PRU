using UnityEngine;

public class PlayerCMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 14f;
    public float dashForce = 20f;
    public float glideFallMultiplier = 0.3f;

    [Header("Wall Jump Settings")]
    public Transform wallCheck;
    public float wallRadius = 0.2f;
    public LayerMask wallLayer;
    private bool isTouchingWall;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;

    private bool canDash = true;
    private bool isDashing;
    private bool isGliding;
    private bool facingRight = true;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Debug.Log("PlayerCMove initialized!");
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallRadius, wallLayer);

        // --- Jump ---
        if (Input.GetButtonDown("Jump"))
        {
            if (isTouchingWall && !isGrounded)
            {
                rb.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * moveSpeed, jumpForce);
                Flip();
            }
            else if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // --- Dash ---
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        // --- Glide ---
        if (Input.GetKey(KeyCode.Space) && !isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * glideFallMultiplier);
            isGliding = true;
        }
        else
        {
            isGliding = false;
        }

        // --- Flip direction ---
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        // --- Animation ---
        if (anim)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isDashing", isDashing);
            anim.SetBool("isGliding", isGliding);
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }

    private System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashForce, 0);
        yield return new WaitForSeconds(0.2f);

        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(1f);
        canDash = true;
    }
}
