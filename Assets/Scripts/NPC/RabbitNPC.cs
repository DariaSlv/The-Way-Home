using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RabbitNPC : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public GameObject continueButton;
    public GameObject interactPrompt;

    [Header("NPC Settings")]
    public string npcName = "Elder Rabbit";
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] dialogueLines = new string[]
    {
        "Oh my! Another lost little one? You look frightened...",
        "Listen carefully, young bunny. This forest is home to dangerous foxes.",
        "If a fox spots you, you have two choices:",
        "Jump on their heads to stun them, or hide quickly in the bushes!",
        "See that bush nearby? Try pressing E to hide. The foxes won't find you there.",
        "The forest provides for us - carrots will heal your wounds...",
        "And if you're lucky, you might find strawberries. They're rare, but restore more health!",
        "Now go, little one. Find your way home safely!"
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
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    // Player components to freeze
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private Vector2 savedVelocity;

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
    }

    void Update()
    {
        if (player == null) return;

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

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (interactPrompt != null && playerInRange)
            interactPrompt.SetActive(true);

        if (freezePlayerDuringDialogue)
        {
            UnfreezePlayer();
        }
    }

    void FreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRb != null)
        {
            savedVelocity = playerRb.velocity;
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