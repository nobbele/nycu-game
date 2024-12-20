using System.Collections.Generic;
using UnityEngine;

// [Serializable]
// public struct BossEnemyStage 
// {
//     public int HealthTrigger;

//     public UnityEvent OnStageEnter;

//     // TODO Make these boss parameters generic over boss type.
//     public NormalDistribution minionSpawnDist;
// }   

// abstract OnStageChanged(T)
// BossEnemy<T>
// GrassBossEnemy : BossEnemy<GrassBossEnemyStage>

public abstract class BossEnemyBase : MonoBehaviour {
    public abstract string BossName { get; }
    public abstract string BossId { get; }
    public abstract int Health { get; protected set; }
    public abstract int MaxHealth { get; protected set; }

    public abstract void ActivateBoss();
}

public abstract class BossEnemy<T> : BossEnemyBase, IDamageHandler
    where T : BossEnemyStage
{
    public override int Health  { get; protected set; } = 100;
    public override int MaxHealth  { get; protected set; } = 100;

    public List<T> Stages = new();

    protected int currentStageIndex = 0;
    public T CurrentStage => Stages[currentStageIndex];

    public T NextStage => currentStageIndex + 1 < Stages.Count ? Stages[currentStageIndex + 1] : null;

    public bool IsDead => Health <= 0;
        public bool IsBossActive { get; private set; } = false;


    public override void ActivateBoss() {
        IsBossActive = true;
        if (CurrentStage != null) CurrentStage.OnStageEnter();
        OnBossActivated();
    }

    public virtual void Update() {
        if (!IsBossActive) return;

    }

    protected virtual void OnBossActivated()  {}


    public void GoToNextStage() {
        if (currentStageIndex + 1 >= Stages.Count) return;

        CurrentStage.IsStageActive = false;

        currentStageIndex += 1;
        Debug.Log($"Stage: #{currentStageIndex + 1}");

        CurrentStage.IsStageActive = true;
        CurrentStage.OnStageEnter();
    }

    public void OnDamage(GameObject source, int damage)
    {
        Health -= damage;

        if (NextStage != null && Health < NextStage.HealthTrigger) {
            GoToNextStage();
        }
    }
}
