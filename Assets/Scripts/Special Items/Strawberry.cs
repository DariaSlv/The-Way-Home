using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strawberry : MonoBehaviour
{
    public float healthIncrease = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SoundManager.instance != null)
                SoundManager.instance.PlayEating();

            HealthBar hb = FindObjectOfType<HealthBar>();
            if (hb != null)
                hb.AddHealth(healthIncrease);

            Destroy(gameObject);
        }
    }
}
