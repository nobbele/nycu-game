using UnityEngine;

public class DefaultEnemyData : BaseEnemyData
{
    public GameObject attackEffect;
    public Vector3 effectScale = Vector3.one;
    public AudioClip attackSound;
    public float effectDuration = 2f;
    public int damageAmount = 10;
}
