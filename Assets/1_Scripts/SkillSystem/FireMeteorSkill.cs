using UnityEngine;

[CreateAssetMenu(fileName = "FireMeteor", menuName = "RPG/Skills/FireMeteor")]
public class FireMeteorSkill : Skill
{
    public GameObject effectInstance;
    private Animator animator;

    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        
        animator.SetTrigger("FireMeteor");
        Vector3 spawnPos = caster.transform.position + caster.transform.forward * 5f - caster.transform.right * 1.25f;
        GameObject effect = Instantiate(effectInstance, spawnPos, Quaternion.identity);
        Destroy(effect, 2f);
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}