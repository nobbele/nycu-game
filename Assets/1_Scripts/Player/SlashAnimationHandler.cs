using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SlashAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private int lastComboIdx;
    private Quaternion rootRotation;
    private bool first;

    public UnityEvent OnSlashPerformed = new();
    
    void Start()
    {
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

        transform.rotation = rootRotation;
        first = true;
    }
}
