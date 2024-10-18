using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    public Transform player;
    public Transform spawnPoint;
    public float chaseRange = 10f;
    public float attackRange = 3f;
    public float wanderRadius = 2.5f;
    public float moveInterval = 3f;
    public float attackInterval = 1.5f;
    public int attackDamage = 10;

    private NavMeshAgent agent;
    private float timeSinceLastMove = 0f;
    private bool isChasingPlayer = false;
    private bool isAttackingPlayer = false;
    private float timeSinceLastAttack = 0f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackRange) {
            isChasingPlayer = false;
            isAttackingPlayer = true;
        }
        else if (distanceToPlayer <= chaseRange) {
            isChasingPlayer = true;
            isAttackingPlayer = false;
        }
        else {
            isChasingPlayer = false;
            isAttackingPlayer = false;
        }

        if (isAttackingPlayer) {
            AttackPlayer();
        }
        else if (isChasingPlayer) {
            ChasePlayer();
        }
        else {
            WanderAroundSpawnPoint();
        }
    }

    void ChasePlayer() {
        agent.SetDestination(player.position);
    }

    void AttackPlayer() {
        agent.SetDestination(transform.position);

        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= attackInterval) {
            if (player.TryGetComponent(out IDamageHandler damageHandler))
                damageHandler.OnDamage(gameObject, attackDamage);
            timeSinceLastAttack = 0f;
        }
    }

    void WanderAroundSpawnPoint() {
        timeSinceLastMove += Time.deltaTime;

        if (timeSinceLastMove >= moveInterval) {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += spawnPoint.position;
            randomDirection.y = spawnPoint.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1)) {
                agent.SetDestination(hit.position);
            }

            timeSinceLastMove = 0f;
        }
    }
}
