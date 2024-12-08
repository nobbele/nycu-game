using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageHandler
{
    public BaseEnemyData enemyData;
    public int Health;
    public bool IsDead => Health <= 0;
    public event Action onDeath;
    public GameObject meshInstance;

    public static void SpawnAt(Vector3 spawnPosition, BaseEnemyData enemyData, Transform spawnerCenter, Action OnEnemyDeath, GameObject spawnEffectPrefab = null) {
        GameObject enemy = new GameObject("Enemy");
        enemy.transform.position = spawnPosition;        

        // Enemy
        Enemy enemyScript = enemy.AddComponent<Enemy>();
        enemyScript.enemyData = enemyData;
        enemyScript.onDeath += OnEnemyDeath;

        // Enemy AI
        BaseEnemyAI enemyAI;
        if (enemyData.enemyAI == null)
        {
            enemyAI = enemy.AddComponent<DefaultEnemyAI>();
        }
        else
        {
            enemyAI = enemy.AddComponent(enemyData.enemyAI.GetType()) as BaseEnemyAI;
        }
        enemyAI.Initialize(GameObject.FindWithTag("Player").transform, spawnerCenter, enemyData);

        // Nav Mesh Agent
        NavMeshAgent navMeshAgent = enemy.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = 3.5f;

        // Collider
        CapsuleCollider collider = enemy.AddComponent<CapsuleCollider>();
        collider.height = 2;

        // Spawning Particle
        if (spawnEffectPrefab != null) {
            var spawnEffect = Instantiate(spawnEffectPrefab);
            spawnEffect.transform.position = spawnPosition + Vector3.up * 1f; 
            spawnEffect.transform.Translate(Vector3.up * 2f);
            spawnEffect.GetComponent<ParticleSystem>().Play();
            spawnEffect.GetComponent<AudioSource>().Play();
        }
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
