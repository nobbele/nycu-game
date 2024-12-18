using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHandler : MonoBehaviour
{
    private MovementController movementController;
    private Animator animator;
    public UnityEvent OnSlashStart = new();
    public UnityEvent OnSlashEnd = new();
    
    void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        animator = GetComponent<Animator>();
    }

    public void StartSlash()
    {
        movementController.ResetComboTimer();
        StartCoroutine(movementController.RotateFacing());
        animator.applyRootMotion = true;
        animator.SetBool("IsSlashing", true);
    }

    public void SlashHitStart()
    {
        OnSlashStart?.Invoke();
    }
    
    public void SlashHitEnd()
    {
        OnSlashEnd?.Invoke();
    }

    public void EndSlash(int index)
    {
        animator.SetBool("IsSlashing", false);
        animator.SetBool($"Slash{index}", false);
        animator.applyRootMotion = false;
        
        transform.rotation = movementController.prevRotation;
    }

    public void StartCast()
    {
        transform.rotation = movementController.prevRotation;
        animator.applyRootMotion = true;
    }

    public void EndCast()
    {
        transform.rotation = movementController.prevRotation;
        animator.applyRootMotion = false;
        animator.SetBool("IsCastingSkill", false);
    }
}
