using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform spawnPoint;
    public EnemyData enemyData;

    private NavMeshAgent agent;
    private Animator animator;
    private Enemy enemy;
    private float timeSinceLastMove = 0f;
    private float timeSinceLastAttack = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        animator = enemy.meshInstance.GetComponent<Animator>();
    }

    void Update()
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

    void ChasePlayer()
    {
        animator.SetBool("IsMoving", true);
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        animator.SetBool("IsMoving", false);

        agent.SetDestination(transform.position);
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= enemyData.attackInterval)
        {
            animator.SetTrigger("Attack");

            if (player.TryGetComponent(out IDamageHandler damageHandler))
            {
                damageHandler.OnDamage(gameObject, enemyData.damageAmount);
                
                if (enemyData.attackEffect != null)
                {
                    Vector3 effectPosition = player.position;
                    if (Physics.Raycast(player.position + Vector3.up, Vector3.down, out RaycastHit hit, 10f))
                    {
                        effectPosition.y = hit.point.y + 0.1f;
                    }
                    else {
                        effectPosition.y = 0f;
                    }
                    GameObject effect = Instantiate(enemyData.attackEffect, effectPosition, Quaternion.identity);
                    Destroy(effect, enemyData.effectDuration);
                }
            }
            timeSinceLastAttack = 0f;
        }
    }

    void WanderAroundSpawnPoint()
    {
        animator.SetBool("IsMoving", true);

        timeSinceLastMove += Time.deltaTime;
        if (timeSinceLastMove >= enemyData.moveInterval)
        {
            Vector3 randomDirection = Random.insideUnitSphere * enemyData.wanderRadius;
            randomDirection += spawnPoint.position;
            randomDirection.y = spawnPoint.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, enemyData.wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }

            timeSinceLastMove = 0f;
        }

        if (agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
