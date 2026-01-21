using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Death Settings")]
    public float fallDeathY = -10f;
    public float deathAnimationDuration = 1f;

    [Header("Respawn")]
    public Transform respawnPoint;
    public string menuSceneName = "MainMenu";

    private bool isDead = false;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;
    private HealthBar healthBar;
    private SpriteRenderer spriteRenderer;
    private DeathScreenUI deathScreenUI;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        healthBar = FindObjectOfType<HealthBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathScreenUI = FindObjectOfType<DeathScreenUI>();

        if (deathScreenUI != null)
        {
            deathScreenUI.SetPlayerDeath(this);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (transform.position.y < fallDeathY)
        {
            Die("fall");
        }

        if (healthBar != null && healthBar.health <= 0)
        {
            Die("health");
        }
    }

    public void Die(string deathType)
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player died from: " + deathType);

        if (SoundManager.instance != null)
            SoundManager.instance.PlayPlayerDeath();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerHealth != null)
            playerHealth.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Death");
        }

        StartCoroutine(HandleDeath());
    }

    IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        if (deathScreenUI != null)
        {
            deathScreenUI.ShowDeathScreen();
        }
        else
        {
            Debug.LogWarning("DeathScreenUI not found! Auto-reloading scene...");
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Respawn()
    {
        isDead = false;

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = Vector3.zero;
            Debug.LogWarning("No respawn point set! Respawning at (0,0,0)");
        }

        if (healthBar != null)
        {
            healthBar.health = healthBar.maxHealth;
            healthBar.DrawHeart();
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.zero;
        }

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerHealth != null)
            playerHealth.enabled = true;

        if (animator != null)
        {
            animator.ResetTrigger("Death");
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
            animator.Play("Idle");
        }
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public bool IsDead()
    {
        return isDead;
    }
}