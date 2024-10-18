using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageHandler
{
    public int Health = 20;

    void Update()
    {
        if (Health <= 0)
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
