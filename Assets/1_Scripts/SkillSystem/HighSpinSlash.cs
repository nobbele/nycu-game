using UnityEngine;

[CreateAssetMenu(fileName = "HighSpinSlash", menuName = "RPG/Skills/HighSpinSlash")]
public class HighSpinSlash : Skill
{
    public GameObject effectInstance;
    private Animator animator;
    
    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        
        animator.SetTrigger("HighSpinSlash");
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}
