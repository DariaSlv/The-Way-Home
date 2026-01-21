using UnityEngine;

public class RemoveOldFadeCanvas : MonoBehaviour
{
    void Start()
    {
        GameObject fadeCanvas = GameObject.Find("FadeCanvas");
        if (fadeCanvas != null)
        {
            Destroy(fadeCanvas);
        }
    }
}