using System.Collections.Generic;

public class BossEnemyStageGrass : BossEnemyStage {
    public NormalDistribution minionSpawnDist;
    public NormalDistribution fireBreathDist;

    public List<EnemyData> enemyData;

    public override void OnStageEnter() {

    }
}