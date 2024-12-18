using UnityEngine;
using System.Collections;

public class DefaultEnemyAI : BaseEnemyAI<DefaultEnemyData>
{
    private AudioSource audioSource;

    public override void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (data is not DefaultEnemyData defaultData)
        {
            Debug.LogError($"DefaultEnemyAI requires DefaultEnemyData, but received {data.GetType().Name}");
            enabled = false;
            return;
        }

        base.Initialize(player, spawnPoint, data);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    protected override void UpdateBehavior()
    {
        ValidateComponents();
        if (!enabled) return;

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
        if (!animator || !animator.isActiveAndEnabled) return;
        
        animator.SetBool("IsMoving", true);
        agent.SetDestination(player.position);
        RotateTowardsPlayer();
    }
    
    private void AttackPlayer()
    {
        if (!animator || !animator.isActiveAndEnabled) return;

        animator.SetBool("IsMoving", false);
        agent.SetDestination(transform.position);
        RotateTowardsPlayer();
        
        if (CheckTimer("attack"))
        {
            animator.SetTrigger("Attack");
            
            if (player.TryGetComponent(out IDamageHandler damageHandler))
            {
                StartCoroutine(ExecuteAttackEffects(damageHandler));
            }
        }
    }
    
    private void WanderAroundSpawnPoint()
    {
        if (!animator || !animator.isActiveAndEnabled) return;

        animator.SetBool("IsMoving", true);
        
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
            animator.SetBool("IsMoving", false);
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private IEnumerator ExecuteAttackEffects(IDamageHandler damageHandler)
    {
        yield return new WaitForSeconds(enemyData.effectDelay);
        
        DisplayAttackEffect(player.position);
        PlayAttackSound();
        damageHandler.OnDamage(gameObject, enemyData.damageAmount);
    }

    private void DisplayAttackEffect(Vector3 position)
    {
        if (enemyData.attackEffect == null) return;

        Vector3 effectPosition = position;
        if (Physics.Raycast(position + Vector3.up, Vector3.down, out RaycastHit hit, 10f))
        {
            effectPosition.y = hit.point.y + 0.1f;
        }
        else
        {
            effectPosition.y = 0f;
        }

        GameObject effect = Instantiate(enemyData.attackEffect, effectPosition, Quaternion.identity);
        effect.transform.localScale = Vector3.Scale(effect.transform.localScale, enemyData.effectScale);
        Destroy(effect, enemyData.effectDuration);
    }

    private void PlayAttackSound()
    {
        if (audioSource != null && enemyData.attackSound != null)
        {
            audioSource.clip = enemyData.attackSound;
            audioSource.Play();
        }
    }
}
