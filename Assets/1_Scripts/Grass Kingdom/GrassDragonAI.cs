using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GrassDragonAI : MonoBehaviour
{
    public GrassDragonState State { get; private set; }

    private BossEnemyGrass bossEnemy;

    public NormalDistribution changeStateDistribution = new();
    public float changeStateTimer = 0;

    void Start()
    {
        bossEnemy = GetComponent<BossEnemyGrass>();
    }

    void Update()
    {
        if (State == GrassDragonState.Dead)
            return;
            
        if (!bossEnemy.IsBossActive)
            State = GrassDragonState.Idle;

        Action updater = State switch
        {
            GrassDragonState.Idle => UpdateIdle,
            GrassDragonState.Wandering => UpdateWandering,
            GrassDragonState.Chasing => UpdateChasing,
            GrassDragonState.AttackingPlayer => UpdateAttackingPlayer,
            GrassDragonState.Summoning => UpdateSummoning,
            GrassDragonState.Dead => () => {},
            _ => throw new NotImplementedException(),
        };

        updater();
    }

    public void SetState(GrassDragonState state)
    {
        Debug.Log($"Changing to state {state}");

        State = state;

        Action starter = State switch
        {
            GrassDragonState.Idle => () => {},
            GrassDragonState.Wandering => StartWandering,
            GrassDragonState.Chasing => StartChasing,
            GrassDragonState.AttackingPlayer => StartAttackingPlayer,
            GrassDragonState.Summoning => StartSummoning,
            GrassDragonState.Dead => () => {},
            _ => throw new NotImplementedException(),
        };

        starter();

    }

    void UpdateIdle()
    {
        // Do Nothing
    }

    void StartWandering()
    {
        changeStateTimer = changeStateDistribution.Sample();
        NewWanderingTarget();
    }

    void NewWanderingTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * bossEnemy.spawnRadius;
        randomDirection += bossEnemy.spawnCenter.position;
        randomDirection.y = bossEnemy.spawnCenter.position.y;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, bossEnemy.spawnRadius, NavMesh.AllAreas))
        {
            StartCoroutine(bossEnemy.Co_SetTarget(hit.position));
        }
    }

    void UpdateWandering()
    {
        if (changeStateTimer > 0)
        {
            changeStateTimer -= Time.deltaTime;
            if (changeStateTimer <= 0)
            {
                changeStateTimer = 0;

                var states = new GrassDragonState[]
                {
                    GrassDragonState.Chasing,
                    GrassDragonState.Summoning
                };
                SetState(states[Random.Range(0, states.Length)]);
            }
        }

        if (bossEnemy.agent.remainingDistance <= bossEnemy.agent.stoppingDistance)
        {
            NewWanderingTarget();
        }
    }

    void StartChasing()
    {
        var target = Player.Instance.transform.position + Random.onUnitSphere * 8f;
        target.y = Player.Instance.transform.position.y;
        StartCoroutine(bossEnemy.Co_SetTarget(target));
    }

    void UpdateChasing()
    {
        if (bossEnemy.agent.remainingDistance <= bossEnemy.agent.stoppingDistance)
        {
            SetState(GrassDragonState.AttackingPlayer);
        }
    }

    bool startedAttack = false;

    void StartAttackingPlayer()
    {
        startedAttack = false;
    }

    void UpdateAttackingPlayer()
    {
        if (startedAttack) return;

        var targetLook = Player.Instance.transform.position;
        targetLook.y = bossEnemy.transform.position.y;
        var targetRot = Quaternion.LookRotation(targetLook - bossEnemy.transform.position);
        bossEnemy.transform.rotation = Quaternion.RotateTowards(bossEnemy.transform.rotation, targetRot, 120 * Time.deltaTime);

        if (Quaternion.Angle(bossEnemy.transform.rotation, targetRot) < 5)
        {
            StartCoroutine(Co_PerformAttack());
        }
    }

    IEnumerator Co_PerformAttack()
    {
        startedAttack = true;
        bossEnemy.BreathFire();
        yield return new WaitForSeconds(5);
        SetState(GrassDragonState.Wandering);
    }

    void StartSummoning()
    {
        StartCoroutine(Co_Summoning());
    }

    IEnumerator Co_Summoning()
    {
        var spawnCount = Random.Range(1, 5);
        for (int i = 0; i < spawnCount; i++)
        {
            bossEnemy.SpawnMinion();
            yield return new WaitForSeconds(Random.Range(1, 3));
        }

        yield return new WaitForSeconds(5);
        SetState(GrassDragonState.Wandering);
    }

    void UpdateSummoning()
    {
        // 
    }
} 