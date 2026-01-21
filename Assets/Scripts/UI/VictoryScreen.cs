using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public TMP_Text victoryTitle;
    public TMP_Text victoryMessage;
    public Button menuButton;

    [Header("Settings")]
    public float fadeInDuration = 1f;
    public string menuSceneName = "MainMenu";

    [Header("Other UI")]
    public GameObject healthBarCanvas;

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (victoryPanel != null)
        {
            canvasGroup = victoryPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = victoryPanel.AddComponent<CanvasGroup>();

            victoryPanel.SetActive(false);
        }

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            if (healthBarCanvas != null)
                healthBarCanvas.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (SoundManager.instance != null)
                SoundManager.instance.PlayWinMusic();

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    void OnMenuClicked()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClick();

            SoundManager.instance.StopWinMusic();

            SoundManager.instance.PlayBackgroundMusic();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    void OnReplayClicked()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}