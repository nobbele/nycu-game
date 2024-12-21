using System;
using UnityEngine;

public class BossSystem : MonoBehaviour
{    
    public GameObject containmentWallsContainer;
    public BossEnemyBase bossEnemy;
    public BossUI ui;
    
    Player player = null;
    bool battleFinished = false;

    [NonSerialized] public static BossSystem Instance;

    // public UnityEvent OnBossStart;

    void Start()
    {
        Instance = this;
        ui.gameObject.SetActive(false);
    }

    void OnBattleStart() {
        Debug.Log("Battle started!!");
        
        if (containmentWallsContainer != null) containmentWallsContainer.SetActive(true);
        if (MusicManager.Instance != null) MusicManager.Instance.PlayBossMusic();

        ui.gameObject.SetActive(true);
        ui.BossName = bossEnemy.BossName;
        bossEnemy.ActivateBoss();
    }

    void OnBattleEnd()
    {
        battleFinished = true;
        ui.gameObject.SetActive(false);

        if (containmentWallsContainer != null) containmentWallsContainer.SetActive(false);
        if (MusicManager.Instance != null) MusicManager.Instance.PlayStageMusic();

        if (string.IsNullOrWhiteSpace(bossEnemy.BossId)) throw new Exception("Boss ID is empty!");
        
        SaveSystem.SetBossClear(bossEnemy.BossId);
    }

    void Update()
    {
        ui.HealthPercent = bossEnemy.Health / (float)bossEnemy.MaxHealth;
        if (bossEnemy.Health <= 0 && !battleFinished)
        {
            OnBattleEnd();
        }
    }


    void OnTriggerEnter(Collider collider) {
        if (this.player != null) return;
        if (battleFinished) return;

        if (collider.TryGetComponent(out Player player)) {
            this.player = player;
            OnBattleStart();
        }
    }
}
