using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public GameObject enemyMesh;
    public RuntimeAnimatorController animatorController;
    public GameObject attackEffect;
    public float effectDuration = 2f;
    public int health = 20;
    public int damageAmount = 10;
    public float attackRange = 3f;
    public float chaseRange = 10f;
    public float attackInterval = 1.5f;
    public float wanderRadius = 2.5f;
    public float moveInterval = 3f;
}
