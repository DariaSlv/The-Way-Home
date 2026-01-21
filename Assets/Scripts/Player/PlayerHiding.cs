using UnityEngine;
using UnityEngine.UI;

public class PlayerHiding : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isHidden = false;
    private bool canHide = false;
    private int originalLayer;

    [Header("UI Settings")]
    public GameObject hidePromptUI; 
    public KeyCode hideKey = KeyCode.E;

    [Header("Layer Settings")]
    public string hiddenLayerName = "HiddenPlayer";

    private GameObject currentBush;
    private Collider2D playerCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        originalLayer = gameObject.layer;

        if (hidePromptUI != null)
            hidePromptUI.SetActive(false);
    }

    void Update()
    {
        if (canHide && Input.GetKeyDown(hideKey))
        {
            if (!isHidden)
            {
                Hide();
            }
            else
            {
                Unhide();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            canHide = true;
            currentBush = collision.gameObject;

            if (hidePromptUI != null && !isHidden)
                hidePromptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bush"))
        {
            canHide = false;
            currentBush = null;

            if (hidePromptUI != null)
                hidePromptUI.SetActive(false);

            if (isHidden)
            {
                Unhide();
            }
        }
    }

    void Hide()
    {
        isHidden = true;
        spriteRenderer.enabled = false;
        int hiddenLayer = LayerMask.NameToLayer(hiddenLayerName);
        if (hiddenLayer != -1)
        {
            gameObject.layer = hiddenLayer;
        }
        if (hidePromptUI != null)
            hidePromptUI.SetActive(false);

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBushSound();
    }

    void Unhide()
    {
        isHidden = false;
        spriteRenderer.enabled = true;
        gameObject.layer = originalLayer;
        if (hidePromptUI != null && canHide)
            hidePromptUI.SetActive(true);

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBushSound();
    }

    public bool IsHidden()
    {
        return isHidden;
    }
}