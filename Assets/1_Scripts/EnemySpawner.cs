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

        Enemy.SpawnAt(spawnPosition, enemyData, spawnCenter, OnEnemyDeath);

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
