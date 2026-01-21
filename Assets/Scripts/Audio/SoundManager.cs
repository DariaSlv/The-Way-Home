using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    [SerializeField] private int additionalSourcesCount = 3;
    private AudioSource[] sfxSources;
    private int currentSourceIndex = 0;

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip warningSound;
    public AudioClip gameOverSound;
    public AudioClip typingSound;
    public AudioClip levelTransitionSound;

    [Header("Player Sounds")]
    public AudioClip jumpSound;
    public AudioClip playerHurtSound;
    public AudioClip playerDeathSound;
    public AudioClip eatingSound;

    [Header("Enemy Sounds")]
    public AudioClip stunEnemySound;

    [Header("Environment Sounds")]
    public AudioClip bushSound;

    [Header("Level Ambience")]
    public AudioClip nightAmbiance;
    public AudioClip forestAmbiance;

    [Header("Typing")]
    private AudioSource typingSource;

    [Header("Music")]
    public AudioClip winMusic;
    public AudioClip backgroundMusic;
    private AudioSource musicSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float defaultSFXVolume = 0.7f;
    [Range(0f, 1f)]
    public float defaultMusicVolume = 0.5f;
    private float sfxVolume;
    private float musicVolume;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();   

        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        musicVolume = PlayerPrefs.GetFloat("WinMusicVolume", defaultMusicVolume);
    }

    private void SetupAudioSources()
    {
        additionalSourcesCount = Mathf.Max(1, additionalSourcesCount);

        sfxSources = new AudioSource[additionalSourcesCount];

        for (int i = 0; i < sfxSources.Length; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.volume = sfxVolume;
            sfxSources[i] = src;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        typingSource = gameObject.AddComponent<AudioSource>();
        typingSource.playOnAwake = false;
        typingSource.loop = true;
        typingSource.volume = sfxVolume;
    }

    public void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableSource();
        if (source == null || !source.enabled || !source.gameObject.activeInHierarchy)
            return;

        source.volume = sfxVolume * volumeMultiplier;
        source.PlayOneShot(clip);
    }

    private AudioSource GetAvailableSource()
    {
        if (sfxSources == null || sfxSources.Length == 0)
        {
            Debug.LogError("SoundManager: SFX pool not initialized!");
            return null;
        }

        for (int i = 0; i < sfxSources.Length; i++)
        {
            int index = (currentSourceIndex + i) % sfxSources.Length;
            if (!sfxSources[index].isPlaying)
            {
                currentSourceIndex = index;
                return sfxSources[index];
            }
        }

        currentSourceIndex = (currentSourceIndex + 1) % sfxSources.Length;
        return sfxSources[currentSourceIndex];
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void PlayWarning()
    {
        PlaySound(warningSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    public void PlayLevelTransition()
    {
        PlaySound(levelTransitionSound);
    }

    public void PlayNightAmbiance()
    {
        if (nightAmbiance == null) return;

        if (musicSource.clip == nightAmbiance && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = nightAmbiance;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayForestAmbiance()
    {
        if (forestAmbiance == null) return;

        if (musicSource.clip == forestAmbiance && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = forestAmbiance;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayTyping()
    {
        if (typingSound == null) return;
        if (typingSource.isPlaying) return;

        typingSource.clip = typingSound;
        typingSource.volume = sfxVolume * 0.6f;
        typingSource.Play();
    }

    public void StopTyping()
    {
        if (typingSource == null) return;
        typingSource.Stop();
    }

    public void PlayJump()
    {
        PlaySound(jumpSound);
    }

    public void PlayPlayerHurt()
    {
        PlaySound(playerHurtSound);
    }

    public void PlayPlayerDeath()
    {
        PlaySound(playerDeathSound);
    }

    public void PlayEating()
    {
        PlaySound(eatingSound);
    }

    public void PlayStunEnemy()
    {
        PlaySound(stunEnemySound);
    }

    public void PlayBushSound()
    {
        PlaySound(bushSound, 0.7f);
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null) return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayWinMusic()
    {
        if (winMusic == null) return;

        if (musicSource.clip == winMusic && musicSource.isPlaying)
            return;

        musicSource.clip = winMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopWinMusic()
    {
        musicSource.Stop();
    }

    public void FadeOutWinMusic(float duration = 1f)
    {
        StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutMusicCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("WinMusicVolume", musicVolume);
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeMultiplier);
    }

    public void StopAllSounds()
    {
        foreach (AudioSource source in sfxSources)
        {
            source.Stop();
        }
        musicSource.Stop();
    }

    public void MuteAll(bool mute)
    {
        foreach (AudioSource source in sfxSources)
        {
            source.mute = mute;
        }
        musicSource.mute = mute;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}