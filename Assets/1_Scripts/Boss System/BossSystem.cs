using UnityEngine;

public class BossSystem : MonoBehaviour
{    
    public GameObject containmentWallsContainer;
    public BossEnemyBase bossEnemy;

    Player player = null;

    // public UnityEvent OnBossStart;

    void OnBattleStart() {
        // OnBossStart.Invoke();
        containmentWallsContainer.SetActive(true);

        Debug.Log("Battle started!!");
        bossEnemy.ActivateBoss();
    }


    void OnTriggerEnter(Collider collider) {
        if (this.player != null) return;

        if (collider.TryGetComponent(out Player player)) {
            this.player = player;
            OnBattleStart();
        }
    }
}
