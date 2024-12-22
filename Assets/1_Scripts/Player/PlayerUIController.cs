using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUD hud;
    [SerializeField] private CharacterPanel characterPanel;
    [SerializeField] private ChestPanel chestPanel;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private MessagePanel messagePanel;
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    
    public CharacterPanel CharacterPanel => characterPanel;
    public ChestPanel ChestPanel => chestPanel;
    public MessagePanel MessagePanel => messagePanel;
    public GameObject PromptPanel => promptPanel;

    private Stack<GameObject> activeUIStack = new Stack<GameObject>();
    private MovementController movementController;
    private InteractionController interactionController;
    
    void Start()
    {
        movementController = GetComponent<MovementController>();
        interactionController = GetComponent<InteractionController>();
        InitializeUIStates();
        SetupCharacterPanel();
        SetupPanels();
    }

    private void SetupPanels()
    {
        if (messagePanel != null)
        {
            messagePanel.Initialize(this);
        }
        
        if (chestPanel != null)
        {
            chestPanel.Initialize(this);
        }
    }
    
    void Update()
    {
        UpdateHUD();
        UpdateAttributePointsText();
        HandleMenuInput();
        UpdateGameState();
    }

    private void InitializeUIStates()
    {
        characterPanel.gameObject.SetActive(false);
        chestPanel.gameObject.SetActive(false);
        messagePanel.gameObject.SetActive(false);
        promptPanel.SetActive(false);
    }
    
    private void UpdateHUD()
    {
        hud.UpdateHealth(playerStats.Health, playerStats.MaxHealth);
        hud.UpdateExperience(playerStats.Experience, playerStats.XpRequired, playerStats.Level);
    }
    
    private void UpdateAttributePointsText()
    {
        if (characterPanel.gameObject.activeSelf)
        {
            characterPanel.AttributePointsLeftText.text = $"Points Left: {playerStats.AttributePoints}";
        }
    }
    
    private void SetupCharacterPanel()
    {
        var attributes = playerStats.GetAttributes();
        
        if (characterPanel.DamageAttributeUI.TryGetComponent<AttributeUI>(out var damageUI))
        {
            damageUI.Initialize(playerStats);
            damageUI.SetValue(attributes.Damage);
        }
        
        if (characterPanel.HealthAttributeUI.TryGetComponent<AttributeUI>(out var healthUI))
        {
            healthUI.Initialize(playerStats);
            healthUI.SetValue(attributes.Health);
        }

        characterPanel.SetSkillPoint(playerStats.SkillPoints);
    }
    
    private void HandleMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleCharacterPanel();
        }
    }

    private void ToggleCharacterPanel()
    {
        if (characterPanel.gameObject.activeSelf)
        {
            HideUI(characterPanel.gameObject);
        }
        else
        {
            ShowUI(characterPanel.gameObject);
        }
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null)
        {
            messagePanel.ShowMessage(message);
            ShowUI(messagePanel.gameObject);
        }
    }

    public void ShowUI(GameObject uiPanel)
    {
        if (!uiPanel.activeSelf)
        {
            uiPanel.SetActive(true);
            activeUIStack.Push(uiPanel);
            UpdateGameState();
        }
    }

    public void HideUI(GameObject uiPanel)
    {
        if (uiPanel.activeSelf)
        {
            uiPanel.SetActive(false);
            
            // Remove the panel from stack
            var tempStack = new Stack<GameObject>();
            while (activeUIStack.Count > 0)
            {
                var panel = activeUIStack.Pop();
                if (panel != uiPanel)
                {
                    tempStack.Push(panel);
                }
            }
            
            // Restore other panels to stack
            while (tempStack.Count > 0)
            {
                activeUIStack.Push(tempStack.Pop());
            }
            
            UpdateGameState();
        }
    }
    
    private void UpdateGameState()
    {
        bool hasActiveUI = activeUIStack.Count > 0;
        
        // Handle game pause
        Time.timeScale = hasActiveUI ? 0f : 1f;
        
        // Handle player movement
        if (movementController != null)
        {
            movementController.DisabledMovement = hasActiveUI;
        }
        
        // Handle interaction
        if (interactionController != null)
        {
            interactionController.SetEnabled(!hasActiveUI);
        }
        
        // Handle camera and cursor
        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.enabled = !hasActiveUI;
        }
        Cursor.lockState = hasActiveUI ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public bool IsUIActive(GameObject uiPanel)
    {
        return activeUIStack.Contains(uiPanel);
    }

    public bool IsAnyUIActive()
    {
        return activeUIStack.Count > 0;
    }
}
