using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Boss : BaseEnemy<BossData>
{
    private BossAI bossAI;

    protected override void InitializeMeshAndAnimator()
    {
        // Ensure mesh instance is set
        if (meshInstance == null)
        {
            meshInstance = transform.Find("Dragon")?.gameObject;
            if (meshInstance == null)
            {
                Debug.LogError("Dragon mesh not found in scene!");
                enabled = false;
                return;
            }
        }

        base.InitializeMeshAndAnimator();
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        InitializeBossAI();
    }

    private void InitializeBossAI()
    {
        if (!TryGetComponent<BossAI>(out bossAI))
        {
            bossAI = gameObject.AddComponent<BossAI>();
        }

        var playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            bossAI.Initialize(playerTransform, transform, enemyData);
        }
    }

    public void PlayIntroduction()
    {
        animator?.SetTrigger("Introduction");
    }
}
