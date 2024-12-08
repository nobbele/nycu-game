using UnityEngine;

public class BaseEnemyData: MonoBehaviour
{
    public GameObject enemyMesh;
    public Vector3 enemyScale = Vector3.one;
    public BaseEnemyAI enemyAI;
    public RuntimeAnimatorController animatorController;
    public int health = 20;
    public float attackRange = 3f;
    public float chaseRange = 10f;
    public float wanderRadius = 2.5f;
    public float attackInterval = 1.5f;
    public float moveInterval = 3f;
}
