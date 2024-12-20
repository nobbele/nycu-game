using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerUIController))]
[RequireComponent(typeof(PlayerCombatSystem))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Inventory))]             
[RequireComponent(typeof(InteractionController))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [SerializeField] private Transform lookHint;
    public Transform LookHint => lookHint;
    
    [SerializeField] private Transform minimapCameraRotationTracker;
    [SerializeField] private Camera minimapCamera;

    private PlayerStats stats;
    private PlayerUIController uiController;
    private PlayerCombatSystem combatSystem;
    private PlayerInputHandler inputHandler;
    private Inventory inventory;
    private InteractionController interaction;
    
    public Inventory PlayerInventory => inventory;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        stats = GetComponent<PlayerStats>();
        uiController = GetComponent<PlayerUIController>();
        combatSystem = GetComponent<PlayerCombatSystem>();
        inputHandler = GetComponent<PlayerInputHandler>();
        inventory = GetComponent<Inventory>(); 
        interaction = GetComponent<InteractionController>();
    }
    
    void Start()
    {
        if (uiController.CharacterPanel != null)
        {
            combatSystem.SetSkillSlots(uiController.CharacterPanel.SkillSlots);
        }
    }
    
    void Update()
    {
        UpdateMinimapCamera();
        
        if (stats.IsDead)
        {
            GameOver();
        }
    }
    
    private void UpdateMinimapCamera()
    {
        if (minimapCamera == null || minimapCameraRotationTracker == null) return;
        
        var targetMinimapRotation = minimapCameraRotationTracker.rotation;
        var targetMinimapRotationEulerAngles = targetMinimapRotation.eulerAngles;
        targetMinimapRotationEulerAngles.x = 90;
        targetMinimapRotation.eulerAngles = targetMinimapRotationEulerAngles;

        minimapCamera.transform.rotation = targetMinimapRotation;
    }
    
    private void GameOver()
    {
        Debug.Log("Game Over");
    }
}
