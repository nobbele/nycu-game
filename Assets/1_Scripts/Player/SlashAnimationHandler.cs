using System.Collections;
using UnityEngine;

public class SlashAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private int lastComboIdx;
    private Quaternion rootRotation;
    private bool first;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartSlash()
    {
        if (first) {
            rootRotation = transform.rotation;
            first = false;
        }
        
        if (lastComboIdx != 0) animator.SetBool($"Slash{lastComboIdx}", false);
        animator.applyRootMotion = true;
        animator.SetBool("IsSlashing", true);
        lastComboIdx++;
    }

    public void EndSlash()
    {
        animator.SetBool($"Slash{lastComboIdx}", false);
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
