using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerCombatSystem : MonoBehaviour
{
    private PlayerStats playerStats;
    private MovementController movementController;
    private AnimationHandler animationHandler;
    private Sword sword;
    
    public List<SkillSlot> SkillSlots { get; private set; }
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        movementController = GetComponent<MovementController>();
        animationHandler = GetComponentInChildren<AnimationHandler>();
        sword = GetComponentInChildren<Sword>();
        
        animationHandler.OnSlashPerformed.AddListener(PerformAttack);
    }

    public void StartAttack()
    {
        StartCoroutine(movementController.SlashComboAnimation());
    }
    
    public void CastSkill(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < SkillSlots.Count && SkillSlots[skillIndex].equippedSkill != null)
        {
            StartCoroutine(movementController.CastSkill(SkillSlots[skillIndex].equippedSkill));
        }
    }
    
    private void PerformAttack()
    {
        var attributes = playerStats.GetAttributes();
        var damage = 10 + 2 * attributes.Damage;
        
        foreach (var damageHandler in sword.OverlappingColliders
                     .Select(collider1 => collider1.GetComponent<IDamageHandler>())
                     .Where(damageHandler => damageHandler != null))
        {

            damageHandler.OnDamage(this.gameObject, damage);
            if (damageHandler.IsDead)
            {
                playerStats.AddExperience(10);
            }
        }
    }
    
    public void SetSkillSlots(List<SkillSlot> skillSlots)
    {
        SkillSlots = skillSlots;
        foreach (var slot in SkillSlots.Where(slot => slot.equippedSkill != null))
        {
            slot.equippedSkill.Reset();
        }
    }
}
