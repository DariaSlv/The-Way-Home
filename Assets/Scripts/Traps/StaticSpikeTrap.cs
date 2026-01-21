using System.Collections;
using UnityEngine;

public class StaticSpikeTrap : MonoBehaviour
{
    [Header("Spike Settings")]
    public bool instantKill = true;
    public int damageAmount = 100; 

    [Header("Visual Feedback")]
    public bool flashOnTouch = true;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Particle Effect")]
    public GameObject deathParticles;
    public bool spawnParticlesOnKill = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();

            if (playerDeath != null && !playerDeath.IsDead())
            {
                if (instantKill)
                {
                    playerDeath.Die("spikes");

                    if (spawnParticlesOnKill && deathParticles != null)
                    {
                        Instantiate(deathParticles, other.transform.position, Quaternion.identity);
                    }
                }
                else
                {
                    PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damageAmount, transform.position);
                    }
                }

                if (flashOnTouch && spriteRenderer != null)
                {
                    StartCoroutine(FlashSpike());
                }
            }
        }
    }

    IEnumerator FlashSpike()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawCube(transform.position, col.bounds.size);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}