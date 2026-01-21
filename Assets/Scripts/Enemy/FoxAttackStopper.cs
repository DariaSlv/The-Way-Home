using System.Collections;
using UnityEngine;

public class FoxAttackStopper : MonoBehaviour
{
    private FoxEnemyAI foxAI;
    private float lastAttackTime = -999f;
    public float attackCooldown = 1.5f;

    void Start()
    {
        foxAI = GetComponentInParent<FoxEnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;

            Vector2 foxPosition = transform.parent != null ? transform.parent.position : transform.position;
            Vector2 playerPosition = other.transform.position;

            if (playerPosition.y > foxPosition.y + 0.5f)
                return;

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.IsInvincible())
            {
                lastAttackTime = Time.time;

                if (foxAI != null)
                    foxAI.StartAttackRecovery();
            }
        }
    }
}