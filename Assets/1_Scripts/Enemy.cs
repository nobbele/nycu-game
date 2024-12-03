using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageHandler
{
    public EnemyData enemyData;
    public int Health;
    public bool IsDead => Health <= 0;
    public event Action onDeath;
    public GameObject meshInstance;

    public static void SpawnAt(Vector3 spawnPosition, EnemyData enemyData, Transform spawnerCenter, Action OnEnemyDeath) {
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.position = spawnPosition;        

        // Enemy
        Enemy enemyScript = enemy.AddComponent<Enemy>();
        enemyScript.enemyData = enemyData;
        enemyScript.onDeath += OnEnemyDeath;

        // Enemy AI
        EnemyAI enemyAI = enemy.AddComponent<EnemyAI>();
        enemyAI.enemyData = enemyData;
        enemyAI.player = GameObject.FindWithTag("Player").transform;
        enemyAI.spawnPoint = spawnerCenter;

        // Nav Mesh Agent
        NavMeshAgent navMeshAgent = enemy.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = 3.5f;

        // Collider
        CapsuleCollider collider = enemy.AddComponent<CapsuleCollider>();
        collider.height = 2;
    }

    void Start()
    {
        Health = enemyData.health;
        if (enemyData.enemyMesh != null)
        {
            meshInstance = Instantiate(enemyData.enemyMesh, transform.position, Quaternion.identity, transform);
            if (meshInstance.TryGetComponent(out Animator animator)) {
                animator.runtimeAnimatorController = enemyData.animatorController;
            }
        }
    }

    void Update()
    {
        if (IsDead)
            OnDead();
    }

    void OnDead()
    {
        onDeath?.Invoke();
        Destroy(gameObject, 2f);
    }

    public void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;

        Health -= damage;
    }
}
