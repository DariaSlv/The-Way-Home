using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxAttackHitbox : MonoBehaviour
{
    public int attackDamage = 2;
    public float attackCooldown = 1f;
    private float lastAttackTime = -999f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time < lastAttackTime + attackCooldown)
                return;

            Vector2 foxPosition = transform.parent != null ? transform.parent.position : transform.position;
            Vector2 playerPosition = other.transform.position;

            if (playerPosition.y > foxPosition.y + 0.5f)
            {
                return;
            }

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (playerHealth.IsInvincible())
                {
                    return;
                }

                playerHealth.TakeDamage(attackDamage, foxPosition);
                lastAttackTime = Time.time;
            }
            else
            {
                HealthBar hb = FindObjectOfType<HealthBar>();
                if (hb != null)
                {
                    hb.AddHealth(-attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}