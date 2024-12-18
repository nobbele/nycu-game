using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BaseEnemy<BaseEnemyData>
{
    public static void SpawnAt(Vector3 position, BaseEnemyData data, Transform center, Action onDeath, GameObject spawnEffect = null)
    {
        // Ensure spawn position is on NavMesh
        if (NavMesh.SamplePosition(position, out NavMeshHit navMeshHit, 5f, NavMesh.AllAreas))
        {
            position = navMeshHit.position;
        }
        else
        {
            Debug.LogWarning($"Spawn position {position} is not on NavMesh. Skipping spawn.");
            return;
        }

        // Ensure enemy is spawned on the ground
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit groundHit, Mathf.Infinity))
        {
            position.y = groundHit.point.y;
        }
        else
        {
            Debug.LogWarning($"Cannot find ground below spawn position {position}. Skipping spawn.");
            return;
        }

        var enemyObj = new GameObject("Enemy");
        enemyObj.transform.position = position;

        // NavMeshAgent
        var navMeshAgent = enemyObj.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = 3.5f;

        // Collider
        var capsuleCollider = enemyObj.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2f;

        // Setup enemy component
        var enemy = enemyObj.AddComponent<Enemy>();
        enemy.enemyData = data;
        enemy.onDeath += onDeath;

        // Create mesh instance
        if (data.enemyMesh != null)
        {
            enemy.meshInstance = Instantiate(data.enemyMesh, position, Quaternion.identity, enemyObj.transform);
            enemy.meshInstance.transform.localScale = Vector3.Scale(
                enemy.meshInstance.transform.localScale, 
                data.enemyScale
            );
        }

        // Initialize enemy first
        enemy.InitializeComponents();

        // Add and initialize AI after enemy is ready
        var playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            Type aiType = data.GetAIType();
            var ai = enemyObj.AddComponent(aiType);
            if (ai is IEnemyAI enemyAI)
            {
                enemyAI.Initialize(playerTransform, center, data);
            }
            else
            {
                Debug.LogError($"AI type {aiType.Name} does not implement IEnemyAI");
            }
        }

        // Create spawn effect if provided
        if (spawnEffect != null)
        {
            var effect = Instantiate(spawnEffect, position + Vector3.up * 3f, Quaternion.identity);
            effect.GetComponent<ParticleSystem>()?.Play();
            effect.GetComponent<AudioSource>()?.Play();
        }
    }
}
