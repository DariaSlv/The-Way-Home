using System.Collections;
using UnityEngine;

public class LampController : MonoBehaviour
{
    [Header("Glow Sprite")]
    public SpriteRenderer glowSprite;
    public Color glowColor = new Color(1f, 0.9f, 0.6f, 0.5f);

    [Header("Glow Settings")]
    public float glowSize = 3f;
    public float glowIntensity = 1f;

    [Header("Sorting Layer")]
    public int sortingOrderOffset = -1;

    [Header("Flicker Effect")]
    public bool enableFlicker = true;
    public float flickerSpeed = 5f;
    public float flickerAmount = 0.1f;
    private float baseAlpha;

    [Header("Pulse Effect")]
    public bool enablePulse = false;
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.2f;

    void Start()
    {
        if (glowSprite == null)
        {
            CreateGlowSprite();
        }
        else
        {
            SetupGlow();
        }
        baseAlpha = glowColor.a;
    }

    void CreateGlowSprite()
    {
        GameObject glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowSprite = glowObject.AddComponent<SpriteRenderer>();
        glowSprite.sprite = CreateCircleSprite();
        SetupGlow();
    }

    void SetupGlow()
    {
        glowSprite.color = glowColor;
        glowSprite.transform.localScale = Vector3.one * glowSize;

        SpriteRenderer parentSprite = GetComponent<SpriteRenderer>();
        if (parentSprite != null)
        {
            glowSprite.sortingLayerName = parentSprite.sortingLayerName;
            glowSprite.sortingOrder = parentSprite.sortingOrder + sortingOrderOffset;
        }
        else
        {
            glowSprite.sortingOrder = sortingOrderOffset;
        }

        glowSprite.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (glowSprite == null) return;

        Color currentColor = glowColor;
        float intensityMod = glowIntensity;

        if (enableFlicker)
        {
            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0) * flickerAmount;
            intensityMod += flicker;
        }

        if (enablePulse)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            intensityMod += pulse;
        }

        currentColor.a = baseAlpha * intensityMod;
        glowSprite.color = currentColor;
    }

    Sprite CreateCircleSprite()
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                float alpha = 1f - (distance / radius);
                alpha = Mathf.Clamp01(alpha);
                alpha = Mathf.Pow(alpha, 2);
                pixels[y * size + x] = new Color(1, 1, 1, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100
        );
    }
}