
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyGrass : BossEnemy<BossEnemyStageGrass>
{
    private float minionSpawnTimer = 0;

    public Transform spawnCenter;
    public float spawnRadius;

    protected override void OnBossActivated()  {
        Debug.Log("Grass!!");
        minionSpawnTimer = CurrentStage.minionSpawnDist.Sample();
    }

    public override void Update() {
        base.Update();
        if (!IsBossActive) return;

        minionSpawnTimer -= Time.deltaTime;
        if (minionSpawnTimer <= 0) {
            minionSpawnTimer = CurrentStage.minionSpawnDist.Sample();
            SpawnMinion();
        }
    }

    void SpawnMinion() {
        Debug.Log("Spawn Minion");
        Vector3 spawnPosition = spawnCenter.position + (Random.insideUnitSphere * spawnRadius);
        spawnPosition.y = spawnCenter.position.y;

        var randomIndex = Random.Range(0, CurrentStage.enemyData.Count);
        Enemy.SpawnAt(spawnPosition, CurrentStage.enemyData[randomIndex], spawnCenter, () => {});
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter.transform.position, spawnRadius);
    }
}