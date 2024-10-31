using UnityEngine;

public interface IDamageHandler
{
    void OnDamage(GameObject source, int damage);
    bool IsDead { get; }
}