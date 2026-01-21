using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryIntroController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject storyPanel;
    public TMP_Text storyText;
    public GameObject skipButton;
    public GameObject continueButton;

    [Header("Story Settings")]
    [TextArea(3, 10)]
    public string[] storyLines = new string[]
    {
        "Deep in the heart of the forest...",
        "A young bunny wandered too far from home.",
        "The trees grew taller, the paths grew darker...",
        "And suddenly, little bunny realized...",
        "He was lost...",
        "Now he must find his way back to the burrow,\nback to his family.",
        "Help him navigate through the forest and return home safely!"
    };

    [Header("Timing")]
    public float typingSpeed = 0.05f;
    public float lineDelay = 1.5f;
    public bool autoAdvance = true;

    [Header("Fade Settings")]
    public Image backgroundImage;
    public float fadeSpeed = 1f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    //private bool storyComplete = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        StopMenuMusic();

        Time.timeScale = 0f;

        if (backgroundImage != null)
        {
            Color blackColor = Color.black;
            blackColor.a = 1f;
            backgroundImage.color = blackColor;
        }

        if (storyPanel != null)
            storyPanel.SetActive(true);

        if (skipButton != null)
        {
            Button btn = skipButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(SkipStory);
            skipButton.SetActive(true);
        }

        if (continueButton != null)
        {
            Button btn = continueButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(NextLine);
            continueButton.SetActive(false);
        }

        StartCoroutine(PlayStory());
    }

    private void StopMenuMusic()
    {
        if (MusicManager.instance != null)
            MusicManager.instance.StopMusic();
    }

    private void PlayLevelMusic()
    {
        SceneMusicTrigger trigger = FindObjectOfType<SceneMusicTrigger>();
        if (trigger != null)
            trigger.PlayMusic();
    }

    IEnumerator PlayStory()
    {
        if (backgroundImage != null)
            yield return StartCoroutine(FadeIn());

        for (currentLineIndex = 0; currentLineIndex < storyLines.Length; currentLineIndex++)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine(storyLines[currentLineIndex]));
            yield return typingCoroutine;

            if (autoAdvance)
            {
                yield return new WaitForSecondsRealtime(lineDelay);
            }
            else
            {
                if (continueButton != null)
                    continueButton.SetActive(true);

                yield return new WaitUntil(() => !isTyping);

                if (continueButton != null)
                    continueButton.SetActive(false);
            }
        }

        yield return new WaitForSecondsRealtime(1f);
        EndStory();
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        storyText.text = "";

        if (SoundManager.instance != null)
            SoundManager.instance.PlayTyping();

        foreach (char letter in line.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        if (SoundManager.instance != null)
            SoundManager.instance.StopTyping();

        isTyping = false;
    }

    void NextLine()
    {

    }

    IEnumerator FadeIn()
    {
        Color bgColor = Color.black;
        bgColor.a = 1f;
        backgroundImage.color = bgColor;

        Color textColor = storyText.color;
        textColor.a = 0f;
        storyText.color = textColor;

        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.unscaledDeltaTime * fadeSpeed;

            textColor.a = alpha;
            storyText.color = textColor;

            yield return null;
        }

        textColor.a = 1f;
        storyText.color = textColor;
    }

    IEnumerator FadeOut()
    {
        Color bgColor = backgroundImage.color;
        Color textColor = storyText.color;

        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.unscaledDeltaTime * fadeSpeed;

            bgColor.a = alpha;
            backgroundImage.color = bgColor;

            textColor.a = alpha;
            storyText.color = textColor;

            yield return null;
        }

        bgColor.a = 0f;
        backgroundImage.color = bgColor;
        textColor.a = 0f;
        storyText.color = textColor;
    }

    public void SkipStory()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (SoundManager.instance != null)
            SoundManager.instance.StopTyping();

        StopAllCoroutines();
        EndStory();
    }

    public void SkipStoryWithSound()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();

        SkipStory();
    }

    void EndStory()
    {
        StartCoroutine(FadeOutAndClose());
    }

    IEnumerator FadeOutAndClose()
    {
        if (skipButton != null)
            skipButton.SetActive(false);

        if (continueButton != null)
            continueButton.SetActive(false);

        if (backgroundImage != null)
            yield return StartCoroutine(FadeOut());

        if (storyPanel != null)
            storyPanel.SetActive(false);

        Time.timeScale = 1f;

        PlayLevelMusic();

        this.enabled = false;
    }
}