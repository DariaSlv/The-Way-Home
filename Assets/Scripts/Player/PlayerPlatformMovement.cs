using UnityEngine;

public class PlayerPlatformMovement : MonoBehaviour
{
    private MovingPlatform currentPlatform;
    private Vector3 lastPlatformPosition;
    private bool isOnPlatform = false;

    [Header("Platform Movement Settings")]
    [Range(0f, 1f)]
    public float horizontalDamping = 0.4f;

    void LateUpdate()
    {
        if (currentPlatform != null && isOnPlatform)
        {
            Vector3 platformDelta = currentPlatform.transform.position - lastPlatformPosition;

            float horizontalInfluence = 1f;

            if (Mathf.Abs(currentPlatform.platformVelocity.x) > Mathf.Abs(currentPlatform.platformVelocity.y))
            {
                horizontalInfluence = horizontalDamping;
            }

            Vector3 movement = new Vector3(
                platformDelta.x * horizontalInfluence,
                platformDelta.y, 
                0
            );

            transform.position += movement;
            lastPlatformPosition = currentPlatform.transform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForPlatform(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckForPlatform(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        MovingPlatform platform = collision.collider.GetComponent<MovingPlatform>();
        if (platform != null && platform == currentPlatform)
        {
            currentPlatform = null;
            isOnPlatform = false;
        }
    }

    void CheckForPlatform(Collision2D collision)
    {
        MovingPlatform platform = collision.collider.GetComponent<MovingPlatform>();

        if (platform == null)
            return;

        bool onTop = false;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                onTop = true;
                break;
            }
        }

        if (onTop)
        {
            if (currentPlatform != platform)
            {
                currentPlatform = platform;
                lastPlatformPosition = platform.transform.position;
            }
            isOnPlatform = true;
        }
        else
        {
            if (currentPlatform == platform)
            {
                currentPlatform = null;
                isOnPlatform = false;
            }
        }
    }
}