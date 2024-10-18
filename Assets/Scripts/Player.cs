using System;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    public int Damage = 0;
    public int Speed = 0;
    public int Stamina = 0;
}
public class Player : MonoBehaviour, IDamageHandler
{

    public int Experience;
    public int Level;

    public int Health = 10;

    public int AttributePoints;

    public PlayerAttributes Attributes;

    public int XpRequired => 20 * Level + 100;

    [SerializeField] private Transform attackRangeHint;

    void Update()
    {
        if (Experience >= XpRequired)
            LevelUp();

        if (Health <= 0)
            GameOver();

        // Input Handlers
        if (Input.GetMouseButtonDown(0))
            Attack();

        // Debug Input
        if (Input.GetKeyDown(KeyCode.U))
            Experience += 10;
    }

    void Attack()
    {
        var rayVector = attackRangeHint.position - transform.position;
        if (Physics.Raycast(
            origin: transform.position,
            direction: rayVector.normalized,
            hitInfo: out RaycastHit hit,
            maxDistance: rayVector.magnitude))
        {
            var gameObject = hit.collider.gameObject;
            if (gameObject.TryGetComponent(out IDamageHandler damageHandler))
            {
                var damage = 1 + Attributes.Damage;
                damageHandler.OnDamage(this.gameObject, damage);
            }
        }
    }

    void LevelUp()
    {
        Experience = 0;
        Level += 1;
        AttributePoints += 1;
        print($"Level Up: {Level - 1} -> {Level}");
        print($"+1 Attribute Point");
        // TODO Update UI
    }

    void GameOver()
    {
        print("Game over");
        // TODO
    }

    public void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;

        Health -= damage;
        // TODO Damage effect
    }
}
