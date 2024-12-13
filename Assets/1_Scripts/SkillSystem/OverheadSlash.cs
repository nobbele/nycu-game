using UnityEngine;

[CreateAssetMenu(fileName = "OverheadSlash", menuName = "RPG/Skills/OverheadSlash")]
public class OverheadSlash : Skill
{
    public GameObject effectInstance;
    private Animator animator;
    
    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        
        animator.SetTrigger("OverheadSlash");

        if (effectInstance != null)
        {
            Vector3 spawnPos = caster.transform.position;
            spawnPos.y += 1;
            GameObject effect = Instantiate(effectInstance, spawnPos, Quaternion.Euler(caster.transform.rotation.eulerAngles - Vector3.up * 15f));
            Destroy(effect, 2);
        }
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}
