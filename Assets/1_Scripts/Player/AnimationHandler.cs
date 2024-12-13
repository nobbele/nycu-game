using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationHandler : MonoBehaviour
{
    private MovementController movementController;
    private Animator animator;
    private int lastComboIdx;
    private Quaternion rootRotation;
    private bool first;

    public UnityEvent OnSlashPerformed = new();
    
    void Start()
    {
        movementController = GetComponentInParent<MovementController>();
        animator = GetComponent<Animator>();
    }

    public void StartSlash()
    {
        if (lastComboIdx > 3) return;

        if (first) {
            rootRotation = transform.rotation;
            first = false;
        }
        if (lastComboIdx != 0) animator.SetBool($"Slash{lastComboIdx}", false);
        animator.applyRootMotion = true;
        animator.SetBool("IsSlashing", true);
        lastComboIdx++;
    }

    public void SlashHit()
    {
        OnSlashPerformed?.Invoke();
    }

    public void EndSlash(int index)
    {
        for (int i = index; i < 4; i++)
        {
            if (i != 0) animator.SetBool($"Slash{i}", false);
        }
        
        if (!animator.GetBool("Slash2") && !animator.GetBool("Slash3"))
        { 
            lastComboIdx = 0;
            animator.SetBool("IsSlashing", false);
            animator.applyRootMotion = false;
        }

        transform.rotation = movementController.prevRotation;
        first = true;
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
