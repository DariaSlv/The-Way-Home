using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    public enum MusicType
    {
        Forest,
        Night
    }

    public MusicType musicType;

    public void PlayMusic()
    {
        if (SoundManager.instance == null)
        {
            Debug.LogError("SoundManager is missing!");
            return;
        }

        switch (musicType)
        {
            case MusicType.Forest:
                SoundManager.instance.PlayForestAmbiance();
                break;

            case MusicType.Night:
                SoundManager.instance.PlayNightAmbiance();
                break;
        }
    }

    public void StopMusic()
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.StopMusic();
    }
}