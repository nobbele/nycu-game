using UnityEngine;
using UnityEngine.AI;

public class BossData : BaseEnemyData
{
    public float screamRange = 12f;
    public float screamInterval = 6f;

    [Header("Attack Damage")]
    public int biteDamage = 10;
    public int flameDamage = 10;
    public int clawDamage = 15;

    [Header("Attack Weights")]
    [Range(0f, 100f)]
    public float biteAttackWeight = 50f;
    [Range(0f, 100f)]
    public float flameAttackWeight = 30f;
    [Range(0f, 100f)]
    public float clawAttackWeight = 20f;
}