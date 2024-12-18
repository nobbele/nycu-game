using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerCombatSystem : MonoBehaviour
{
    private PlayerStats playerStats;
    private MovementController movementController;
    private AnimationHandler animationHandler;
    private Sword sword;
    
    private List<Collider> collectedColliders = new List<Collider>();
    private bool collectingColliders = false;
    
    public List<SkillSlot> SkillSlots { get; private set; }
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        movementController = GetComponent<MovementController>();
        animationHandler = GetComponentInChildren<AnimationHandler>();
        sword = GetComponentInChildren<Sword>();
        
        animationHandler.OnSlashStart.AddListener(StartColliderCollection);
        animationHandler.OnSlashEnd.AddListener(EndColliderCollection);
    }

    void Update()
    {
        if (collectingColliders)
        {
            var newColliders = sword.OverlappingColliders.Except(collectedColliders).ToList();
            collectedColliders.AddRange(newColliders);

            foreach (var newCollider in newColliders)
            {
                if (newCollider.TryGetComponent(out IDamageHandler damageHandler))
                    PerformAttack(damageHandler);
            }
        }
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

    private void StartColliderCollection()
    {
        collectedColliders.Clear();
        collectingColliders = true;
    }

    private void EndColliderCollection()
    {
        collectingColliders = false;
    }
    
    private void PerformAttack(IDamageHandler damageHandler)
    {
        var attributes = playerStats.GetAttributes();
        var damage = 10 + 2 * attributes.Damage;
        
        damageHandler.OnDamage(this.gameObject, damage);
        if (damageHandler.IsDead)
        {
            playerStats.AddExperience(10);
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
