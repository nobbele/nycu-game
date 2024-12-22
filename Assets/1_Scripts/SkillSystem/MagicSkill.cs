using UnityEngine;

public abstract class MagicSkill : Skill
{
    protected void AddTriggerToEffect(GameObject effect)
    {
        // Rigidbody
        var rb = effect.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Collider
        CapsuleCollider collider = effect.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = effect.AddComponent<CapsuleCollider>();
        }
        collider.height = 5f;
        collider.radius = 1f;
        collider.isTrigger = true;

        // Trigger
        SkillEffectTrigger trigger = effect.GetComponent<SkillEffectTrigger>();
        if (trigger == null)
        {
            trigger = effect.AddComponent<SkillEffectTrigger>();
        }
        trigger.Initialize(damage);
    }
}
