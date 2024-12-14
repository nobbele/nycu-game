using System;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    public int Damage = 0;
    public int Speed = 0;
    public int Stamina = 0;
    public int Health = 0;
}

public class PlayerStats : MonoBehaviour, IDamageHandler
{
    [SerializeField] private PlayerAttributes attributes;
    
    public int Experience = 0;
    public int Level = 0;
    public int Health = 100;
    public float InvincibilityTime = 1f;
    public int AttributePoints = 0;
    public int SkillPoints = 0;
    
    public int MaxHealth => 100 + 20 * attributes.Health;
    private float invincibilityTimer = 0;
    public bool IsInvincible => invincibilityTimer > 0;
    public bool IsDead => Health <= 0;
    public int XpRequired => 20 * Level + 100;
    
    void Update()
    {
        if (Experience >= XpRequired)
        {
            LevelUp();
        }

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            invincibilityTimer = Mathf.Max(0, invincibilityTimer);
        }
    }
    
    public void ModifyAttribute(string attributeName, int value)
    {
        if (AttributePoints <= 0) return;
        
        var field = typeof(PlayerAttributes).GetField(attributeName);
        if (field != null)
        {
            field.SetValue(attributes, (int)field.GetValue(attributes) + value);
            AttributePoints--;
        }
    }
    
    public void AddExperience(int amount)
    {
        Experience += amount;
    }
    
    private void LevelUp()
    {
        Experience = 0;
        Level += 1;
        AttributePoints += 1;
        SkillPoints += 1;
    }
    
    public void OnDamage(GameObject source, int damage)
    {
        if (source == this.gameObject) return;
        if (IsInvincible) return;

        Health -= damage;
        invincibilityTimer = InvincibilityTime;
    }
    
    public PlayerAttributes GetAttributes() => attributes;
}
