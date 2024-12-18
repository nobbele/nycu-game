using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : BaseEnemyAI<BossData>
{
    private bool wasOutsideScreamRange = true;
    private bool isAnimating;

    protected override void InitializeTimers()
    {
        base.InitializeTimers();
        AddTimer("scream", enemyData.screamInterval);
    }

    protected override void UpdateBehavior()
    {
        if (isAnimating || animator == null) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        
        if (distanceToPlayer <= enemyData.attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= enemyData.screamRange)
        {
            ScreamAtPlayer();
        }
        else
        {
            WanderAroundSpawnPoint();
            wasOutsideScreamRange = true;
        }
    }

    private void ScreamAtPlayer()
    {
        LookAtPlayer();
        animator.SetBool("IsWalking", false);
        agent.SetDestination(transform.position);

        if (wasOutsideScreamRange || CheckTimer("scream"))
        {
            isAnimating = true;
            animator.SetTrigger("Scream");
            wasOutsideScreamRange = false;
        }
    }

    private void AttackPlayer()
    {
        LookAtPlayer();
        animator.SetBool("IsWalking", false);
        agent.SetDestination(transform.position);

        if (CheckTimer("attack"))
        {
            isAnimating = true;
            PerformWeightedAttack();
        }
    }

    private void WanderAroundSpawnPoint()
    {
        animator.SetBool("IsWalking", true);

        if (CheckTimer("move"))
        {
            Vector3 randomDirection = Random.insideUnitSphere * enemyData.wanderRadius;
            randomDirection += spawnPoint.position;
            randomDirection.y = spawnPoint.position.y;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, enemyData.wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void PerformWeightedAttack()
    {
        float totalWeight = enemyData.biteAttackWeight + enemyData.flameAttackWeight + enemyData.clawAttackWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (player.TryGetComponent(out IDamageHandler damageHandler))
        {
            if (randomValue < enemyData.biteAttackWeight)
            {
                animator.SetTrigger("BiteAttack");
                damageHandler.OnDamage(gameObject, enemyData.biteDamage);
            }
            else if (randomValue < enemyData.biteAttackWeight + enemyData.flameAttackWeight)
            {
                animator.SetTrigger("FlameAttack");
                StartCoroutine(ExecuteFlameAttack(damageHandler));
            }
            else
            {
                animator.SetTrigger("ClawAttack");
                damageHandler.OnDamage(gameObject, enemyData.clawDamage);
            }
        }
    }

    private IEnumerator ExecuteFlameAttack(IDamageHandler damageHandler)
    {
        if (TryGetComponent(out DragonFireManager fireManager))
        {
            yield return new WaitForSeconds(0.7f);
            fireManager.StartFire();
            damageHandler.OnDamage(gameObject, enemyData.flameDamage);
            
            yield return new WaitForSeconds(1.1f);
            fireManager.StopFire();
        }
    }

    public void OnAnimationEnd()
    {
        isAnimating = false;
    }
}
