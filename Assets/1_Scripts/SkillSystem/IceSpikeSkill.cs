using UnityEngine;

[CreateAssetMenu(fileName = "IceSpike", menuName = "RPG/Skills/IceSpike")]
public class IceSpikeSkill : Skill
{
    public GameObject effectInstance;
    private Animator animator;

    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        
        animator.SetTrigger("IceSpike");
        Vector3 spawnPos = caster.transform.position + caster.transform.forward * 3f - caster.transform.right * 1.25f;
        GameObject effect = Instantiate(effectInstance, spawnPos, Quaternion.identity);
        effect.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
        Destroy(effect, 2f);
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}
