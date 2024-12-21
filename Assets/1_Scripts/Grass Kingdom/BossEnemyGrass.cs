
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BossEnemyGrass : BossEnemy<BossEnemyStageGrass>
{
    public override string BossName => "Qaivnaenoss, Queen of the Forest";
    
    public const string BOSS_ID = "GrassKingdom";
    public override string BossId => BOSS_ID;

    public Transform spawnCenter;
    public float spawnRadius;

    #region Unity Objects
    public ParticleSystem fireBreathingParticleSystem;
    public AudioSource fireBreathingSoundEffect;
    public GameObject spawnEffectPrefab;
    
    [NonSerialized] public Animator animator;
    [NonSerialized] public NavMeshAgent agent;
    #endregion

    public Vector3 fireBreathHalfExtents;
    public int fireBreathDamage;

    private GrassDragonAI dragonAi;
    public bool animationBlock;

    private Vector3 targetPosition;

    void Start()
    {
        dragonAi = GetComponent<GrassDragonAI>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    protected override void OnBossActivated() 
    {
        Scream();
        dragonAi.SetState(GrassDragonState.Wandering);
    }

    public IEnumerator Co_SetTarget(Vector3 targetPosition)
    {
        yield return null;
        // yield return new WaitUntil(() => !animationBlock);
        agent.SetDestination(targetPosition);
        this.targetPosition = targetPosition;
    }

    public void OnAnimationEnd()
    {
        Debug.Log("OnAnimationEnd");
        animationBlock = false;
    }

    public void Scream()
    {
        animationBlock = true;
        animator.SetTrigger("Scream");
    }

    IEnumerator Co_Die()
    {
        dragonAi.SetState(GrassDragonState.Dead);
        dragonAi.enabled = false;
        agent.enabled = false;
        
        animator.Play("Die");
        // TODO particle system

        yield return new WaitForSeconds(4);

        Destroy(gameObject);
    }

    public override void Update() {
        base.Update();

        var agentVelocity = agent.velocity.magnitude;
        animator.SetBool("IsWalking", agentVelocity > 0 && agentVelocity <= 4);
        animator.SetBool("IsRunning", agentVelocity > 4);

        if (dragonAi.State != GrassDragonState.Dead && Health <= 0)
        {
            Health = 0;
            StartCoroutine(Co_Die());
        }

        if (!IsBossActive) return;

        // minionSpawnTimer -= Time.deltaTime;
        // if (minionSpawnTimer <= 0) {
        //     minionSpawnTimer = CurrentStage.minionSpawnDist.Sample();
        //     SpawnMinion();
        // }

        // fireBreathTimer -= Time.deltaTime;
        // if (fireBreathTimer <= 0) {
        //     fireBreathTimer = CurrentStage.fireBreathDist.Sample();
        //     BreathFire();
        // }
    }

    public void BreathFire() {
        fireBreathingParticleSystem.Play();
        fireBreathingSoundEffect.Play();

        animationBlock = true;
        animator.SetTrigger("FlameAttack");

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

    public void SpawnMinion() {
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
    
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1f);
    }
}