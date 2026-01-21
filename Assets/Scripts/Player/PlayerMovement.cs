using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = true;
    private bool isGrounded = false;
    private int jumpCount = 0;
    public int maxJump = 2;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Enemy Bounce")]
    public float enemyBounceForce = 8f;
    public LayerMask enemyLayer;

    [Header("Platform Settings")]
    public float platformDownSpeedThreshold = 0.5f; 
    private float lastGroundedTime = 0f;
    private float coyoteTime = 0.1f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        animator.SetBool("isRunning", Mathf.Abs(move) > 0.1f);

        if (move > 0 && !isFacingRight)
            Flip();
        else if (move < 0 && isFacingRight)
            Flip();

        bool wasGrounded = isGrounded;
        isGrounded = CheckGrounded();

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            bool canCoyoteJump = Time.time - lastGroundedTime < coyoteTime;

            if (isGrounded || canCoyoteJump || jumpCount < maxJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount++;

                if (SoundManager.instance != null)
                    SoundManager.instance.PlayJump();
            }
        }

        animator.SetBool("isJumping", !isGrounded);
    }

    bool CheckGrounded()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);

        foreach (Collider2D hit in hits)
        {
            MovingPlatform platform = hit.GetComponent<MovingPlatform>();

            if (platform != null)
            {
                if (platform.platformVelocity.y < -platformDownSpeedThreshold)
                {
                    continue;
                }
            }

            return true;
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.7f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, enemyBounceForce);
                    jumpCount = 0;

                    if (SoundManager.instance != null)
                        SoundManager.instance.PlayJump();

                    return;
                }
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}