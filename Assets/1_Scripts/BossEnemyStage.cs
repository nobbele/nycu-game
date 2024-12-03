using UnityEngine;

public class BossEnemyStage : MonoBehaviour {
    public float HealthTrigger;

    public bool IsStageActive = false;
    
    public virtual void OnStageEnter() { }
}