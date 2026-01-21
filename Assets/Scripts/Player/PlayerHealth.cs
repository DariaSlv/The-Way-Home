using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.5f;
    private bool isInvincible = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    private bool isKnockedBack = false;

    [Header("Visual Feedback")]
    public float flashInterval = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("References")]
    private HealthBar healthBar;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerDeath playerDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDeath = GetComponent<PlayerDeath>();
        healthBar = FindObjectOfType<HealthBar>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Vector2 enemyPosition)
    {
        if (isInvincible)
            return;
        if (playerDeath != null && playerDeath.IsDead())
            return;

        if (SoundManager.instance != null)
            SoundManager.instance.PlayPlayerHurt();

        if (healthBar != null)
            healthBar.AddHealth(-damage);
        StartCoroutine(InvincibilityCoroutine());
        StartCoroutine(KnockbackCoroutine(enemyPosition));
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            }
            yield return new WaitForSeconds(flashInterval);

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            yield return new WaitForSeconds(flashInterval);

            elapsed += flashInterval * 2;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isInvincible = false;
    }

    IEnumerator KnockbackCoroutine(Vector2 enemyPosition)
    {
        isKnockedBack = true;

        Vector2 knockbackDirection = ((Vector2)transform.position - enemyPosition).normalized;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;

            Vector2 knockbackVelocity = new Vector2(
                knockbackDirection.x * knockbackForce,
                knockbackForce * 0.7f
            );

            rb.velocity = knockbackVelocity;
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (playerMovement != null)
            playerMovement.enabled = true;

        isKnockedBack = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}