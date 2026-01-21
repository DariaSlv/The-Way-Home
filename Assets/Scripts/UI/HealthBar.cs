using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public float health, maxHealth;

    List<HealthHeart> hearts = new List<HealthHeart>();

    private void Start()
    {
        DrawHeart();
    }

    public void AddHealth(float amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
        if (health < 0)
            health = 0;

        DrawHeart();
    }

    public void DrawHeart()
    {
        ClearHearts();

        int heartsToDraw = Mathf.CeilToInt(maxHealth / 2f);

        for (int i = 0; i < heartsToDraw; i++)
            CreateEmptyHeart();

        for (int i = 0; i < hearts.Count; i++)
        {
            float currentHeartHealth = Mathf.Clamp(health - i * 2, 0, 2);

            if (currentHeartHealth >= 2f)
                hearts[i].SetHeartImg(HeartStatus.Full);
            else if (currentHeartHealth >= 1f)
                hearts[i].SetHeartImg(HeartStatus.Half);
            else
                hearts[i].SetHeartImg(HeartStatus.Empty);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform, false);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();  
        heartComponent.SetHeartImg(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        hearts = new List<HealthHeart>();   
    }
}