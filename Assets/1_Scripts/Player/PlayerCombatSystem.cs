using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerCombatSystem : MonoBehaviour
{
    [SerializeField] private Transform attackRaycastHint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRaycastRadius = 0.5f;
    
    private PlayerStats playerStats;
    private MovementController movementController;
    private AnimationHandler animationHandler;
    
    public List<SkillSlot> SkillSlots { get; private set; }
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        movementController = GetComponent<MovementController>();
        animationHandler = GetComponentInChildren<AnimationHandler>();
        
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
        if (Physics.SphereCast(
            origin: attackRaycastHint.position,
            direction: transform.forward,
            radius: attackRaycastRadius,
            hitInfo: out RaycastHit hit,
            maxDistance: attackRange
        ))
        {
            if (hit.collider.gameObject.TryGetComponent(out IDamageHandler damageHandler))
            {
                var attributes = playerStats.GetAttributes();
                var damage = 10 + 2 * attributes.Damage;
                damageHandler.OnDamage(this.gameObject, damage);

                if (damageHandler.IsDead)
                {
                    playerStats.AddExperience(10);
                }
            }
        }
    }
    
    public void SetSkillSlots(List<SkillSlot> skillSlots)
    {
        SkillSlots = skillSlots;
        foreach (var slot in SkillSlots)
        {
            if (slot.equippedSkill != null)
            {
                slot.equippedSkill.Reset();
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!attackRaycastHint) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            attackRaycastHint.position, 
            attackRaycastHint.position + transform.forward * attackRange
        );
        Gizmos.DrawWireSphere(
            attackRaycastHint.position + transform.forward * attackRange, 
            attackRaycastRadius
        );
    }
}
