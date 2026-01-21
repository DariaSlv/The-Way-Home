using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;

    [Header("Menu Music")]
    public AudioClip menuMusic;
    public bool playOnAwake = true;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        audioSource.volume = savedVolume;

        if (playOnAwake && menuMusic != null)
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (audioSource.clip == menuMusic && audioSource.isPlaying)
            return;

        audioSource.clip = menuMusic;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}