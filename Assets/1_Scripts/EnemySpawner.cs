using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public EnemyData enemyData;
    public Transform spawnCenter;
    public float spawnRadius = 5f;
    public float spawnInterval = 3f;
    public int maxEnemies = 3;

    private int currentEnemies = 0;
    private float timeSinceLastSpawn = 0f;

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = spawnCenter.position + (Random.insideUnitSphere * spawnRadius);
        spawnPosition.y = spawnCenter.position.y;

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
        enemyAI.spawnPoint = spawnCenter;

        // Nav Mesh Agent
        NavMeshAgent navMeshAgent = enemy.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = 3.5f;

        // Collider
        CapsuleCollider collider = enemy.AddComponent<CapsuleCollider>();
        collider.height = 2;

        currentEnemies++;
    }

    void OnEnemyDeath()
    {
        currentEnemies--;
    }

        void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter.transform.position, spawnRadius);
    }
}
