using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MamaRabbitNPC : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public GameObject continueButton;
    public GameObject interactPrompt;

    [Header("Victory Screen")]
    public VictoryScreen victoryScreen;

    [Header("NPC Settings")]
    public string npcName = "Mama Rabbit";
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] dialogueLines = new string[]
    {
        "My baby! You're home! I was so worried about you!",
        "You're so brave, my little one. Welcome home!",
        "Let's get you inside where it's warm and safe."
    };

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;
    public bool useTypingEffect = true;

    [Header("Audio")]
    public bool playTypingSound = true;

    [Header("Player Freeze")]
    public bool freezePlayerDuringDialogue = true;

    private Transform player;
    private bool playerInRange = false;
    private bool isDialogueActive = false;
    private bool hasCompletedDialogue = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    private Animator animator;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerRb = playerObj.GetComponent<Rigidbody2D>();
            playerAnimator = playerObj.GetComponent<Animator>();
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (continueButton != null)
        {
            Button btn = continueButton.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(NextLine);
        }

        if (nameText != null)
            nameText.text = npcName;

        animator = GetComponent<Animator>();

        if (victoryScreen == null)
            victoryScreen = FindObjectOfType<VictoryScreen>();
    }

    void Update()
    {
        if (player == null || hasCompletedDialogue) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (interactPrompt != null && !isDialogueActive)
                    interactPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(interactKey) && !isDialogueActive)
            {
                StartDialogue();
            }

            if (isDialogueActive && Input.GetKeyDown(interactKey))
            {
                if (isTyping)
                {
                    SkipTyping();
                }
                else
                {
                    NextLine();
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (interactPrompt != null)
                    interactPrompt.SetActive(false);
            }
        }

        if (playerInRange && !isDialogueActive)
        {
            FacePlayer();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        currentLineIndex = 0;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (animator != null)
            animator.SetBool("isTalking", true);

        if (freezePlayerDuringDialogue)
        {
            FreezePlayer();
        }

        DisplayLine();
    }

    void DisplayLine()
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        string line = dialogueLines[currentLineIndex];

        if (useTypingEffect)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine(line));
        }
        else
        {
            dialogueText.text = line;
        }

        if (continueButton != null)
            continueButton.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        if (continueButton != null)
            continueButton.SetActive(false);

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;

            if (playTypingSound && SoundManager.instance != null)
                SoundManager.instance.PlayTyping();

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (continueButton != null)
            continueButton.SetActive(true);
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        dialogueText.text = dialogueLines[currentLineIndex];

        if (continueButton != null)
            continueButton.SetActive(true);
    }

    void NextLine()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.PlayButtonClick();

        currentLineIndex++;
        DisplayLine();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        hasCompletedDialogue = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (animator != null)
            animator.SetBool("isTalking", false);

        if (freezePlayerDuringDialogue)
        {
            UnfreezePlayer();
        }

        StartCoroutine(ShowVictoryAfterDelay(1f));
    }

    IEnumerator ShowVictoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (victoryScreen != null)
        {
            victoryScreen.ShowVictory();
        }
    }

    void FreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isJumping", false);
        }
    }

    void UnfreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isJumping", false);
        }
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}