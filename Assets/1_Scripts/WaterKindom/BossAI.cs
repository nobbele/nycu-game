using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : BaseEnemyAI
{
    protected BossData bossData => enemyData as BossData;
    private bool wasOutsideScreamRange = true;
    private bool isAnimating = false;

    public override void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (data is not BossData)
        {
            Debug.LogError($"BossAI requires BossData, but received {data.GetType().Name}");
            enabled = false;
            return;
        }

        base.Initialize(player, spawnPoint, data);
    }

    protected override void InitializeTimers()
    {
        if (bossData == null) return;
        
        base.InitializeTimers();
        AddTimer("scream", bossData.screamInterval);
    }

    protected override void UpdateBehavior()
    {
        if (isAnimating) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Look at player if in range
        if (distanceToPlayer <= bossData.screamRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (distanceToPlayer > bossData.screamRange)
        {
            wasOutsideScreamRange = true;
        }

        if (distanceToPlayer <= enemyData.attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= bossData.screamRange)
        {
            ScreamAtPlayer();
        }
        else
        {
            WanderAroundSpawnPoint();
        }
    }

    private void AttackPlayer()
    {
        animator.SetBool("IsWalking", false);
        agent.SetDestination(transform.position);

        if (CheckTimer("attack"))
        {
            isAnimating = true;
            PerformWeightedAttack();
        }
    }

    private void ScreamAtPlayer()
    {
        animator.SetBool("IsWalking", false);
        agent.SetDestination(transform.position);

        if (wasOutsideScreamRange || CheckTimer("scream"))
        {
            isAnimating = true;
            animator.SetTrigger("Scream");
            wasOutsideScreamRange = false;
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

    private void PerformWeightedAttack()
    {
        float totalWeight = bossData.biteAttackWeight + bossData.flameAttackWeight + bossData.clawAttackWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (player.TryGetComponent(out IDamageHandler damageHandler))
        {
            if (randomValue < bossData.biteAttackWeight)
            {
                animator.SetTrigger("BiteAttack");
                damageHandler.OnDamage(gameObject, bossData.biteDamage);
            }
            else if (randomValue < bossData.biteAttackWeight + bossData.flameAttackWeight)
            {
                animator.SetTrigger("FlameAttack");
                if (TryGetComponent(out DragonFireManager fireManager))
                {
                    StartCoroutine(DelayedFireStart(fireManager, 0.7f));
                }
                damageHandler.OnDamage(gameObject, bossData.flameDamage);
            }
            else
            {
                animator.SetTrigger("ClawAttack");
                damageHandler.OnDamage(gameObject, bossData.clawDamage);
            }
        }
    }

    public void OnAnimationEnd()
    {
        isAnimating = false;
    }

    private IEnumerator DelayedFireStart(DragonFireManager fireManager, float delay)
    {
        yield return new WaitForSeconds(delay);
        fireManager.StartFire();
        
        float remainingTime = 2f - delay - 0.2f;
        yield return new WaitForSeconds(remainingTime);
        fireManager.StopFire();
    }
}
