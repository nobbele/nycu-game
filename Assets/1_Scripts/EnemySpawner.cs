using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public Transform spawnCenter;

    public float spawnRadius = 5f;
    public float spawnInterval = 3f;
    public int maxEnemies = 3;

    private int currentEnemies = 0;
    private float timeSinceLastSpawn = 0f;

    void Update() {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnInterval && currentEnemies < maxEnemies) {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnEnemy() {
        Vector3 spawnPosition = spawnCenter.position + (Random.insideUnitSphere * spawnRadius);
        spawnPosition.y = spawnCenter.position.y;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null) {
            enemyAI.spawnPoint = spawnCenter;
            enemyAI.player = GameObject.FindWithTag("Player").transform;
        }
        currentEnemies++;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter.transform.position, spawnRadius);
    }
}
