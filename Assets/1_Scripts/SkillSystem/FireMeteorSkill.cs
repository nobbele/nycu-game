using UnityEngine;

[CreateAssetMenu(fileName = "FireMeteor", menuName = "RPG/Skills/FireMeteor")]
public class FireMeteorSkill : MagicSkill
{
    public GameObject effectInstance;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip soundEffect;

    protected override void Execute(GameObject caster)
    {
        if (animator == null)
            animator = caster.GetComponentInChildren<Animator>();
        if (audioSource == null)
            audioSource = caster.GetComponentInChildren <AudioSource>();
        
        animator.SetTrigger("FireMeteor");
        audioSource.clip = soundEffect;
        audioSource.Play();
        Vector3 spawnPos = caster.transform.position + caster.transform.forward * 5f - caster.transform.right * 1.25f;
        GameObject effect = Instantiate(effectInstance, spawnPos, Quaternion.identity);
        AddTriggerToEffect(effect);
        Destroy(effect, 2f);
        
        Debug.Log($"{skillName} cast by {caster.name}");
    }
}