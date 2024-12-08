using System.Collections.Generic;

public class BossEnemyStageGrass : BossEnemyStage {
    public NormalDistribution minionSpawnDist;
    public NormalDistribution fireBreathDist;

    public List<DefaultEnemyData> enemyData;

    public override void OnStageEnter() {

    }
}