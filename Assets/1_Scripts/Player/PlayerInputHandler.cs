using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerStats playerStats;
    private PlayerUIController uiController;
    private PlayerCombatSystem combatSystem;
    
    private bool IsMenuOpen => uiController.IsAnyMenuOpen;
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        uiController = GetComponent<PlayerUIController>();
        combatSystem = GetComponent<PlayerCombatSystem>();
    }
    
    void Update()
    {
        if (!IsMenuOpen)
        {
            HandleCombatInput();
            HandleDebugInput();
        }
    }
    
    private void HandleCombatInput()
    {
        // Handle basic attack
        if (Input.GetMouseButtonDown(0))
        {
            combatSystem.StartAttack();
        }
        
        // Handle skill casts
        for (int i = 0; i < combatSystem.SkillSlots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                combatSystem.CastSkill(i);
            }
        }
    }
    
    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            playerStats.AddExperience(10);
        }
    }
}
