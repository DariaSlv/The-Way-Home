using UnityEngine;

public class Lvl2MusicTrigger : MonoBehaviour
{
    public AudioClip gameMusic;

    void Start()
    {
        if (MusicManager.instance == null)
        {
            Debug.LogError("MusicManager is missing!");
            return;
        }

        if (gameMusic == null)
        {
            Debug.LogWarning("No music clip assigned to SceneMusicTrigger.");
            return;
        }

        MusicManager.instance.StopMusic();

        MusicManager.instance.PlayMusic(gameMusic);
    }
}