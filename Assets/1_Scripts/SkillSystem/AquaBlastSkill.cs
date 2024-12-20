using UnityEngine;

[CreateAssetMenu(fileName = "AquaBlast", menuName = "RPG/Skills/AquaBlast")]
public class AquaBlastSkill : Skill
{
    public GameObject effectInstance;
    private Animator animator;

    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        
        animator.SetTrigger("AquaBlast");
        Vector3 spawnPos = caster.transform.position + caster.transform.forward * 2f - caster.transform.right * 1.25f;
        GameObject effect = Instantiate(effectInstance, spawnPos, Quaternion.identity);
        effect.transform.localScale = new Vector3(2f, 2f, 2f);
        Destroy(effect, 2f);
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}
