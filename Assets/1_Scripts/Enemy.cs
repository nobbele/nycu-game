using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageHandler
{
    public EnemyData enemyData;
    public int Health;
    public bool IsDead => Health <= 0;
    public event Action onDeath;
    public GameObject meshInstance;

    void Start()
    {
        Health = enemyData.health;
        if (enemyData.enemyMesh != null)
        {
            meshInstance = Instantiate(enemyData.enemyMesh, transform.position, Quaternion.identity, transform);
            Animator animator = meshInstance.GetComponent<Animator>();
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
    }

    void Update()
    {
        if (IsDead)
            OnDead();
    }

    void OnDead()
    {
        onDeath?.Invoke();
        Destroy(gameObject, 2f);
    }

    public void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;

        Health -= damage;
    }
}
