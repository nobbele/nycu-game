using UnityEngine;

public class DefaultEnemyData : BaseEnemyData
{
    public GameObject attackEffect;
    public Vector3 effectScale = Vector3.one;
    public AudioClip attackSound;
    public float effectDuration = 2f;
    public float effectDelay = 0.1f;
    public int damageAmount = 10;

    public override System.Type GetAIType() => typeof(DefaultEnemyAI);
}
