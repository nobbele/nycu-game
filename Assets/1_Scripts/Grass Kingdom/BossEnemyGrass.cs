
using UnityEngine;

public class BossEnemyGrass : BossEnemy<BossEnemyStageGrass>
{
    private float minionSpawnTimer = 0;
    private float fireBreathTimer = 0;

    public Transform spawnCenter;
    public float spawnRadius;

    public ParticleSystem fireBreathingParticleSystem;
    public AudioSource fireBreathingSoundEffect;
    public GameObject spawnEffectPrefab;

    public Vector3 fireBreathHalfExtents;
    public int fireBreathDamage;

    protected override void OnBossActivated()  {
        Debug.Log("Grass!!");
        minionSpawnTimer = CurrentStage.minionSpawnDist.Sample();
        fireBreathTimer = CurrentStage.fireBreathDist.Sample();
    }

    public override void Update() {
        base.Update();
        if (!IsBossActive) return;

        minionSpawnTimer -= Time.deltaTime;
        if (minionSpawnTimer <= 0) {
            minionSpawnTimer = CurrentStage.minionSpawnDist.Sample();
            SpawnMinion();
        }

        fireBreathTimer -= Time.deltaTime;
        if (fireBreathTimer <= 0) {
            fireBreathTimer = CurrentStage.fireBreathDist.Sample();
            BreathFire();
        }
    }

    void BreathFire() {
        fireBreathingParticleSystem.Play();
        fireBreathingSoundEffect.Play();

        foreach (var hitInfo in Physics.BoxCastAll(
            fireBreathingParticleSystem.transform.position + transform.forward * fireBreathHalfExtents.z / 2f, 
            fireBreathHalfExtents, 
            transform.forward
        ))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamageHandler damageHandler))
            {
                damageHandler.OnDamage(gameObject, fireBreathDamage);
            }
        }
    }

    void SpawnMinion() {
        Debug.Log("Spawn Minion");
        Vector3 spawnPosition = spawnCenter.position + (Random.insideUnitSphere * spawnRadius);
        spawnPosition.y = spawnCenter.position.y;

        var randomIndex = Random.Range(0, CurrentStage.enemyData.Count);
        Enemy.SpawnAt(spawnPosition, CurrentStage.enemyData[randomIndex], spawnCenter, () => {}, spawnEffectPrefab);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter.transform.position, spawnRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(fireBreathingParticleSystem.transform.position + transform.forward * fireBreathHalfExtents.z / 2f, fireBreathHalfExtents);
    }
}