using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageHandler
{
    public int Health = 20;

    public bool IsDead => Health <= 0;

    void Update()
    {
        if (IsDead)
            OnDead();
    }

    void OnDead()
    {
        // TODO Death effect
        Destroy(gameObject);
    }

    public void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;

        Health -= damage;
        // TODO i-frames
        // TODO Damage effect
    }
}
