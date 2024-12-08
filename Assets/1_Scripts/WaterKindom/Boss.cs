using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Boss : Enemy
{
    private void Awake()
    {
        // Find the existing mesh in scene
        meshInstance = transform.Find("Dragon").gameObject;
        if (meshInstance == null)
        {
            Debug.LogError("Dragon mesh not found in scene!");
            return;
        }
    }

    protected void Start()
    {
        // Set initial health
        Health = enemyData.health;

        // Setup animator if needed
        if (meshInstance.TryGetComponent(out Animator animator) && enemyData.animatorController != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }

        // Initialize AI after mesh is setup
        if (TryGetComponent(out BossAI bossAI))
        {
            bossAI.Initialize(
                GameObject.FindWithTag("Player").transform,
                transform,
                enemyData
            );
        }
    }

    public void PlayIntroduction()
    {
        if (meshInstance.TryGetComponent(out Animator animator))
        {
            animator.SetTrigger("Introduction");
        }
    }
}
