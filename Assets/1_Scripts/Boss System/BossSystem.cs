using UnityEngine;

public class BossSystem : MonoBehaviour
{    
    public GameObject containmentWallsContainer;
    public BossEnemyBase bossEnemy;
    public BossUI ui;

    Player player = null;

    // public UnityEvent OnBossStart;

    void Start()
    {
        ui.gameObject.SetActive(false);
    }

    void OnBattleStart() {
        // OnBossStart.Invoke();
        containmentWallsContainer.SetActive(true);

        Debug.Log("Battle started!!");
        MusicManager.Instance.PlayBossMusic();
        ui.gameObject.SetActive(true);
        ui.BossName = bossEnemy.BossName;
        bossEnemy.ActivateBoss();
    }

    void Update()
    {
        ui.HealthPercent = bossEnemy.Health / (float)bossEnemy.MaxHealth;
    }


    void OnTriggerEnter(Collider collider) {
        if (this.player != null) return;

        if (collider.TryGetComponent(out Player player)) {
            this.player = player;
            OnBattleStart();
        }
    }
}
