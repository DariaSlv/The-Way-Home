using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "Level 2";

    [Header("Transition Settings")]
    public float transitionDuration = 1f;
    public Color fadeColor = Color.black;

    [Header("Audio")]
    public AudioClip transitionSound;

    private bool isTransitioning = false;
    private static GameObject fadeCanvas;
    private static CanvasGroup fadeCanvasGroup;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionToNextLevel());
        }
    }

    IEnumerator TransitionToNextLevel()
    {
        isTransitioning = true;

        if (SoundManager.instance != null)
        {
            SoundManager.instance.StopMusic();         
            SoundManager.instance.PlayNightAmbiance(); 
            SoundManager.instance.PlaySound(transitionSound);
        }

        Rigidbody2D playerRb = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.isKinematic = true;
        }

        if (fadeCanvas == null)
        {
            CreateFadeCanvas();
        }

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / transitionDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        while (!asyncLoad.isDone)
            yield return null;

        if (fadeCanvas == null)
        {
            CreateFadeCanvas();
        }
        else
        {
            fadeCanvasGroup = fadeCanvas.GetComponentInChildren<CanvasGroup>();
        }

        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / transitionDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f;

        Destroy(fadeCanvas);
        fadeCanvas = null;
        fadeCanvasGroup = null;
    }

    void CreateFadeCanvas()
    {
        fadeCanvas = new GameObject("FadeCanvas");
        DontDestroyOnLoad(fadeCanvas);

        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        UnityEngine.UI.CanvasScaler scaler = fadeCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        fadeCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject panel = new GameObject("FadePanel");
        panel.transform.SetParent(fadeCanvas.transform, false);

        UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = fadeColor;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;

        fadeCanvasGroup = panel.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)box.offset, box.size);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}