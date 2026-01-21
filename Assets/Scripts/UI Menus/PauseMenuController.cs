using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject pauseMenuPanel;

    [Header("Audio")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("Graphics")]
    public TMP_Dropdown qualityDropdown;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Player Control")]
    public MonoBehaviour playerControllerScript;
    public Rigidbody2D playerRigidbody;

    [Header("UI Elements to Hide")]
    public GameObject healthBarCanvas;

    private bool isPaused = false;
    private Camera mainCamera;
    private RenderTexture pauseScreenshot;

    void Start()
    {
        mainCamera = Camera.main;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        LoadSettings();

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (pauseScreenshot != null)
        {
            pauseScreenshot.Release();
            pauseScreenshot = null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (pauseScreenshot != null)
        {
            pauseScreenshot.Release();
            pauseScreenshot = null;
        }

        if (SoundManager.instance != null)
            SoundManager.instance.StopAllSounds();

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBackgroundMusic();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        int savedQuality = PlayerPrefs.GetInt("QualityLevel", 1);

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            UpdateVolumeText(savedVolume);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.value = savedQuality;
        }

        AudioListener.volume = savedVolume;
        QualitySettings.SetQualityLevel(savedQuality);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
            musicManager.SetVolume(value);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    void UpdateVolumeText(float value)
    {
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        if (pauseScreenshot != null)
        {
            pauseScreenshot.Release();
        }
    }

    public void PlayButtonClick()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();
    }

    public void ResumeGameWithSound()
    {
        PlayButtonClick();
        ResumeGame();
    }

    public void ReturnToMainMenuWithSound()
    {
        PlayButtonClick();
        ReturnToMainMenu();
    }
}