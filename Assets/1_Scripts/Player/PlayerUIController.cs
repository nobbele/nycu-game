using UnityEngine;
using Cinemachine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private HUD hud;
    [SerializeField] private CharacterMenu characterMenu;
    [SerializeField] private SkillPanel skillPanel;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    public SkillPanel SkillPanel => skillPanel;
    public InventoryUI InventoryUI => inventoryUI;
    public GameObject PromptPanel => promptPanel;

    public bool IsAnyMenuOpen => 
        characterMenu.gameObject.activeInHierarchy || 
        skillPanel.gameObject.activeInHierarchy ||
        inventoryUI.gameObject.activeInHierarchy;
    
    private MovementController movementController;
    
    void Start()
    {
        movementController = GetComponent<MovementController>();

        // Initialize UI states
        characterMenu.gameObject.SetActive(false);
        skillPanel.gameObject.SetActive(false);
        inventoryUI.gameObject.SetActive(false);
        promptPanel.SetActive(false);

        SetupCharacterMenu();
        SetupSkillPanel();
    }
    
    void Update()
    {
        UpdateHUD();
        UpdateCharacterMenu();
        HandleMenuState();
        UpdateCursorState();
    }
    
    private void UpdateHUD()
    {
        hud.UpdateHealth(playerStats.Health, playerStats.MaxHealth);
        hud.UpdateExperience(playerStats.Experience, playerStats.XpRequired, playerStats.Level);
    }
    
    private void UpdateCharacterMenu()
    {
        characterMenu.AttributePointsLeftText.text = $"Points Left: {playerStats.AttributePoints}";
        characterMenu.DamageAttributeUI.SetEnabled(playerStats.AttributePoints > 0);
        characterMenu.HealthAttributeUI.SetEnabled(playerStats.AttributePoints > 0);
    }
    
    private void SetupCharacterMenu()
    {
        var attributes = playerStats.GetAttributes();
        
        characterMenu.DamageAttributeUI.Title = "Damage";
        characterMenu.DamageAttributeUI.Value = attributes.Damage;
        characterMenu.HealthAttributeUI.Title = "Health";
        characterMenu.HealthAttributeUI.Value = attributes.Health;
    }
    
    private void SetupSkillPanel()
    {
        if (skillPanel != null)
        {
            skillPanel.SetSkillPoint(playerStats.SkillPoints);
        }
    }
    
    private void HandleMenuState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            characterMenu.gameObject.SetActive(!characterMenu.gameObject.activeInHierarchy);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Cursor.lockState = CursorLockMode.None;
            skillPanel.gameObject.SetActive(!skillPanel.gameObject.activeInHierarchy);
        }
        
        movementController.DisabledMovement = IsAnyMenuOpen;
    }
    
    private void UpdateCursorState()
    {
        cinemachineFreeLook.enabled = !IsAnyMenuOpen;
        Cursor.lockState = IsAnyMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
