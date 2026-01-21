using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Audio")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("Graphics")]
    public TMP_Dropdown qualityDropdown;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject mainMenuPanel;

    void Start()
    {
        LoadSettings();

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
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

        Debug.Log("Quality set to: " + QualitySettings.names[qualityIndex]);
    }

    public void BackToMainMenu()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
}