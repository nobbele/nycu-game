using UnityEngine;

public class DefaultEnemyAI : BaseEnemyAI
{
    protected DefaultEnemyData defaultEnemyData => enemyData as DefaultEnemyData;

    public override void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (data is not DefaultEnemyData)
        {
            Debug.LogError($"DefaultEnemyAI requires DefaultEnemyData, but received {enemyData.GetType().Name}");
            enabled = false;
            return;
        }

        base.Initialize(player, spawnPoint, data);
    }

    protected override void UpdateBehavior()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= enemyData.attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= enemyData.chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            WanderAroundSpawnPoint();
        }
    }
    
    private void ChasePlayer()
    {
        if (animator != null) animator.SetBool("IsMoving", true);
        agent.SetDestination(player.position);
    }
    
    private void AttackPlayer()
    {
        if (animator != null) animator.SetBool("IsMoving", false);
        agent.SetDestination(transform.position);
        
        if (CheckTimer("attack"))
        {
            if (animator != null) animator.SetTrigger("Attack");
            
            if (player.TryGetComponent(out IDamageHandler damageHandler))
            {
                DisplayAttackEffect(player.position);
                damageHandler.OnDamage(gameObject, enemyData.health);
            }
        }
    }
    
    private void WanderAroundSpawnPoint()
    {
        if (animator != null) animator.SetBool("IsMoving", true);
        
        if (CheckTimer("move"))
        {
            Vector3 randomDirection = Random.insideUnitSphere * enemyData.wanderRadius;
            randomDirection += spawnPoint.position;
            randomDirection.y = spawnPoint.position.y;
            
            if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out UnityEngine.AI.NavMeshHit hit, enemyData.wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }
        
        if (agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            if (animator != null) animator.SetBool("IsMoving", false);
        }
    }

    private void DisplayAttackEffect(Vector3 position)
    {
        if (defaultEnemyData.attackEffect == null)
        {
            return;
        }

        Vector3 effectPosition = position;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hit, 10f))
        {
            effectPosition.y = hit.point.y + 0.1f;
        }
        else
        {
            effectPosition.y = 0f;
        }

        GameObject effect = Instantiate(defaultEnemyData.attackEffect, effectPosition, Quaternion.identity);
        Destroy(effect, defaultEnemyData.effectDuration);
    }
}
