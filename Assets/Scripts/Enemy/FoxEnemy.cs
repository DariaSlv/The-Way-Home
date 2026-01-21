using System.Collections;
using UnityEngine;

public class FoxEnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    public float speed = 1f;
    private int currentPatrolIndex = 0;
    private bool facingRight = true;
    private Rigidbody2D rb;
    private Animator animator;
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public int attackDamage = 2;
    public float attackCooldown = 1f;
    private float lastAttackTime;
    public float flipDeadband = 0.05f;
    public float flipCooldown = 0.2f;
    private float lastFlipTime;
    private bool isAttacking = false;
    public float proximityRange = 3f;
    private bool isWaitingAtEdge = false;

    private PlayerHiding playerHiding;
    private bool isConfused = false;
    public float confusionDuration = 2f;

    [Header("Patrol Pause")]
    public float patrolPauseInterval = 4f;
    public float patrolPauseDuration = 2f;

    private float patrolPauseTimer;
    private bool isPausingPatrol = false;

    [Header("Stun Settings")]
    public float stunDuration = 2f;
    private bool isStunned = false;
    public Color stunColor = new Color(0.7f, 0.7f, 1f, 1f);
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private bool isInAttackRecovery = false;

    [Header("Chase Warning")]
    private bool isChasing = false;
    private bool hasPlayedWarning = false;

    void Start()
    {
        patrolPauseTimer = patrolPauseInterval;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            playerHiding = player.GetComponent<PlayerHiding>();

        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogError($"[{gameObject.name}] Nu are suficiente patrol points setate! Trebuie minim 2.");
            enabled = false;
            return;
        }

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
            {
                Debug.LogError($"[{gameObject.name}] Patrol Point [{i}] este NULL!");
                enabled = false;
                return;
            }
        }

        Debug.Log($"[{gameObject.name}] Initialized with patrol from X:{patrolPoints[0].position.x} to X:{patrolPoints[1].position.x}");
    }

    void Update()
    {
        if (player == null || patrolPoints.Length == 0) return;

        if (isStunned || isInAttackRecovery)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);

            if (isChasing)
            {
                isChasing = false;
                hasPlayedWarning = false;
            }

            return;
        }

        bool playerIsHidden = playerHiding != null && playerHiding.IsHidden();

        if (isConfused)
        {
            if (isChasing)
            {
                isChasing = false;
                hasPlayedWarning = false;
            }

            if (!isWaitingAtEdge)
            {
                Patrol();
                animator.SetBool("isRunning", rb.velocity.x != 0);
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float dirToPlayer = player.position.x - transform.position.x;
        bool playerInFront = (dirToPlayer > 0 && facingRight) || (dirToPlayer < 0 && !facingRight);

        float minX = Mathf.Min(patrolPoints[0].position.x, patrolPoints[1].position.x);
        float maxX = Mathf.Max(patrolPoints[0].position.x, patrolPoints[1].position.x);
        bool playerInsidePatrol = player.position.x >= minX && player.position.x <= maxX;

        if (playerIsHidden && distanceToPlayer <= chaseRange && !isConfused)
        {
            if (isChasing)
            {
                isChasing = false;
                hasPlayedWarning = false;
            }

            StartCoroutine(BecomeConfused());
            return;
        }

        if (!playerIsHidden && playerInsidePatrol && distanceToPlayer <= chaseRange)
        {
            if (!isChasing)
            {
                isChasing = true;

                if (!hasPlayedWarning)
                {
                    if (SoundManager.instance != null)
                        SoundManager.instance.PlayWarning();
                    hasPlayedWarning = true;
                }
            }

            if (!playerInFront)
            {
                FlipWithChecks(dirToPlayer);
            }

            ChaseDuringPatrol();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                hasPlayedWarning = false;
            }

            if (!isWaitingAtEdge)
            {
                Patrol();
            }
        }

        if (!isAttacking && !isWaitingAtEdge && !isConfused)
            animator.SetBool("isRunning", rb.velocity.x != 0);

        animator.SetBool("isAttacking", isAttacking);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.7f)
                {
                    if (!isStunned)
                    {
                        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                        if (playerHealth != null && !playerHealth.IsInvincible())
                        {
                            StartCoroutine(ApplyStun());
                        }
                    }
                    return;
                }
            }
        }
    }

    IEnumerator ApplyStun()
    {
        isStunned = true;
        isAttacking = false;
        isConfused = false;

        if (SoundManager.instance != null)
            SoundManager.instance.PlayStunEnemy();

        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);

        if (spriteRenderer != null)
            spriteRenderer.color = stunColor;

        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    IEnumerator BecomeConfused()
    {
        if (isConfused) yield break;

        isConfused = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        isAttacking = false;

        yield return new WaitForSeconds(confusionDuration);

        isConfused = false;
    }

    void Patrol()
    {
        if (isWaitingAtEdge || isPausingPatrol) return;

        patrolPauseTimer -= Time.deltaTime;
        if (patrolPauseTimer <= 0f)
        {
            StartCoroutine(PatrolPause());
            patrolPauseTimer = patrolPauseInterval;
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        float dir = target.position.x - transform.position.x;

        if (Mathf.Abs(dir) < 0.1f)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            StartCoroutine(WaitAndFlipAtEdge());
            return;
        }

        rb.velocity = new Vector2(Mathf.Sign(dir) * speed, rb.velocity.y);
        FlipWithChecks(dir);
    }

    IEnumerator PatrolPause()
    {
        isPausingPatrol = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);

        yield return new WaitForSeconds(patrolPauseDuration);

        isPausingPatrol = false;
    }

    IEnumerator WaitAndFlipAtEdge()
    {
        isWaitingAtEdge = true;
        yield return new WaitForSeconds(0.5f);
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isWaitingAtEdge = false;
    }

    void ChaseDuringPatrol()
    {
        float dir = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(dir) * speed * 3f, rb.velocity.y);
        if (Mathf.Abs(dir) > flipDeadband)
            FlipWithChecks(dir);
    }

    void FlipWithChecks(float horizontalDir)
    {
        if (Time.time < lastFlipTime + flipCooldown)
            return;
        if ((horizontalDir > 0 && !facingRight) || (horizontalDir < 0 && facingRight))
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            lastFlipTime = Time.time;
        }
    }

    public void StartAttackRecovery()
    {
        if (!isInAttackRecovery)
            StartCoroutine(AttackRecoveryCoroutine());
    }

    IEnumerator AttackRecoveryCoroutine()
    {
        isInAttackRecovery = true;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.8f);

        isInAttackRecovery = false;
    }

    void OnDrawGizmos()
    {
        if (patrolPoints != null && patrolPoints.Length >= 2)
        {
            if (patrolPoints[0] != null && patrolPoints[1] != null)
            {
                float minX = Mathf.Min(patrolPoints[0].position.x, patrolPoints[1].position.x);
                float maxX = Mathf.Max(patrolPoints[0].position.x, patrolPoints[1].position.x);
                float y = transform.position.y;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(minX, y - 1, 0), new Vector3(minX, y + 1, 0));
                Gizmos.DrawLine(new Vector3(maxX, y - 1, 0), new Vector3(maxX, y + 1, 0));
                Gizmos.DrawLine(new Vector3(minX, y, 0), new Vector3(maxX, y, 0));

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, chaseRange);
            }
        }
    }
}
