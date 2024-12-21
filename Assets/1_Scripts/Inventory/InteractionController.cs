using UnityEngine;
using TMPro;

[RequireComponent(typeof(Player))]
public class InteractionController : MonoBehaviour
{
    private IInteractable currentInteractable;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float checkRadius = 3f;
    
    private Player player;
    private PlayerUIController uiController;
    private GameObject promptPanel;
    
    private float checkTimer;
    private bool isEnabled = true;

    private void Start()
    {
        player = GetComponent<Player>();
        uiController = GetComponent<PlayerUIController>();
        promptPanel = uiController.PromptPanel;
    }

    private void Update()
    {
        if (!isEnabled || uiController.IsAnyUIActive())
        {
            HidePrompt();
            return;
        }

        checkTimer += Time.deltaTime;
        
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0;
            CheckForInteractable();
        }

        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.OnInteract(gameObject);
        }

        UpdatePromptUI();
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (!enabled)
        {
            currentInteractable = null;
            HidePrompt();
        }
    }

    private void CheckForInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, interactableLayer);
        
        IInteractable bestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                if (!interactable.CanInteract(gameObject))
                    continue;

                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestInteractable = interactable;
                }
            }
        }

        currentInteractable = bestInteractable;
    }

    private void UpdatePromptUI()
    {
        if (promptPanel == null) return;

        if (currentInteractable != null)
        {
            ShowPrompt(currentInteractable.GetInteractPrompt());
        }
        else
        {
            HidePrompt();
        }
    }

    private void ShowPrompt(string text)
    {
        promptPanel.SetActive(true);
        var promptText = promptPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (promptText != null)
        {
            promptText.text = text;
        }
    }

    private void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}
