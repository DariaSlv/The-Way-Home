using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    public GameObject deathPanel;
    public TMP_Text deathText;
    public Button respawnButton;
    public Button menuButton;

    [Header("Settings")]
    public float fadeInDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private PlayerDeath playerDeath;

    void Start()
    {
        if (deathPanel != null)
        {
            canvasGroup = deathPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = deathPanel.AddComponent<CanvasGroup>();

            deathPanel.SetActive(false);
        }

        if (respawnButton != null)
            respawnButton.onClick.AddListener(OnRespawnClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void SetPlayerDeath(PlayerDeath pd)
    {
        playerDeath = pd;
    }

    public void ShowDeathScreen()
    {
        if (deathPanel != null)
        {
            if (SoundManager.instance != null)
                SoundManager.instance.PlayGameOver();

            deathPanel.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

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

    void OnRespawnClicked()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        ResetAllObjects();

        if (playerDeath != null)
            playerDeath.Respawn();
    }

    private void ResetAllObjects()
    {
        ResettableObject[] objects = FindObjectsOfType<ResettableObject>();
        foreach (var obj in objects)
            obj.ResetObject();
    }

    void OnMenuClicked()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();

        Time.timeScale = 1f;

        if (SoundManager.instance != null)
            SoundManager.instance.StopAllSounds();

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBackgroundMusic();

        if (playerDeath != null)
            playerDeath.GoToMenu();
        else
        {
            Debug.LogWarning("PlayerDeath reference not set! Loading scene 0 as fallback.");
            SceneManager.LoadScene(0);
        }
    }
}