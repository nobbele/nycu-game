using UnityEngine;
using Cinemachine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUD hud;
    [SerializeField] private CharacterPanel characterPanel;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    
    public CharacterPanel CharacterPanel => characterPanel;
    public InventoryUI InventoryUI => inventoryUI;
    public GameObject PromptPanel => promptPanel;

    public bool IsAnyMenuOpen => characterPanel.gameObject.activeSelf || 
                                inventoryUI.gameObject.activeSelf;
    
    private MovementController movementController;
    
    void Start()
    {
        movementController = GetComponent<MovementController>();
        InitializeUIStates();
        SetupCharacterPanel();
    }
    
    void Update()
    {
        UpdateHUD();
        UpdateAttributePointsText();
        HandleMenuState();
        UpdateCursorState();
    }

    private void InitializeUIStates()
    {
        characterPanel.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
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
    
    private void HandleMenuState()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleCharacterPanel();
        }
        
        movementController.DisabledMovement = IsAnyMenuOpen;
    }

    private void ToggleCharacterPanel()
    {
        bool newState = !characterPanel.gameObject.activeSelf;
        characterPanel.gameObject.SetActive(newState);
        
        if (newState)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    
    private void UpdateCursorState()
    {
        cinemachineFreeLook.enabled = !IsAnyMenuOpen;
        Cursor.lockState = IsAnyMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
