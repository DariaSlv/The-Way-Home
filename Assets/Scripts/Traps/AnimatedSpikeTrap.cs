using System.Collections;
using UnityEngine;

public class AnimatedSpikeTrap : MonoBehaviour
{
    [Header("Spike Settings")]
    public bool instantKill = true;
    public int damageAmount = 100;

    [Header("Animation Settings")]
    public bool useAnimation = true;
    public float timeUp = 2f;      
    public float timeDown = 2f;     
    public float transitionSpeed = 3f; 

    [Header("Movement Settings")]
    public float extendedHeight = 1f;  
    private Vector3 startPosition;
    private Vector3 extendedPosition;
    private bool isExtended = false;
    private bool isTransitioning = false;

    [Header("Visual Feedback")]
    public bool flashOnTouch = true;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Warning")]
    public bool showWarning = true;
    public GameObject warningIndicator; 
    public float warningTime = 0.5f;

    [Header("Collision")]
    public BoxCollider2D spikeCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (spikeCollider == null)
            spikeCollider = GetComponent<BoxCollider2D>();

        startPosition = transform.position;
        extendedPosition = startPosition + Vector3.up * extendedHeight;

        if (warningIndicator != null)
            warningIndicator.SetActive(false);

        if (useAnimation)
        {
            StartCoroutine(SpikeAnimationCycle());
        }
        else
        {
            transform.position = extendedPosition;
            isExtended = true;
        }
    }

    IEnumerator SpikeAnimationCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeDown);

            if (showWarning)
            {
                if (warningIndicator != null)
                    warningIndicator.SetActive(true);

                yield return new WaitForSeconds(warningTime);

                if (warningIndicator != null)
                    warningIndicator.SetActive(false);
            }

            yield return StartCoroutine(ExtendSpikes());

            yield return new WaitForSeconds(timeUp);

            yield return StartCoroutine(RetractSpikes());
        }
    }

    IEnumerator ExtendSpikes()
    {
        isTransitioning = true;
        isExtended = false;

        while (Vector3.Distance(transform.position, extendedPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                extendedPosition,
                transitionSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = extendedPosition;
        isExtended = true;
        isTransitioning = false;
    }

    IEnumerator RetractSpikes()
    {
        isTransitioning = true;

        while (Vector3.Distance(transform.position, startPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                transitionSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = startPosition;
        isExtended = false;
        isTransitioning = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isExtended && !isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();

            if (playerDeath != null && !playerDeath.IsDead())
            {
                if (instantKill)
                {
                    playerDeath.Die("spikes");
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

    void OnTriggerStay2D(Collider2D other)
    {
        if (isExtended && other.CompareTag("Player"))
        {
            PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();
            if (playerDeath != null && !playerDeath.IsDead() && instantKill)
            {
                playerDeath.Die("spikes");
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
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 extended = start + Vector3.up * extendedHeight;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f); 
        Gizmos.DrawCube(start, Vector3.one * 0.5f);

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); 
        Gizmos.DrawCube(extended, Vector3.one * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, extended);
    }
}