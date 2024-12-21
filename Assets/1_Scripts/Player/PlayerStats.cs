using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    [SerializeField] private int _damage;
    public int Damage
    {
        get => _damage;
        set => SaveAttribute("Damage", _damage = value);
    }
    
    [SerializeField] private int _speed;
    public int Speed
    {
        get => _speed;
        set => SaveAttribute("Speed", _speed = value);
    }
    
    [SerializeField] private int _stamina;
    public int Stamina
    {
        get => _stamina;
        set => SaveAttribute("Stamina", _stamina = value);
    }
    
    [SerializeField] private int _health;
    public int Health
    {
        get => _health;
        set => SaveAttribute("Health", _health = value);
    }
    
    private static void SaveAttribute(string key, int value)
    {
        PlayerPrefs.SetInt($"Attribute_{key}", value);
        if (value != 0) SaveSystem.HasSave = true;
    }
    
    private static int LoadAttribute(string key, int defaultValue = default)
    {
        return PlayerPrefs.GetInt($"Attribute_{key}", defaultValue);
    }
    
    public static void ResetSavedAttributes()
    {
        SaveAttribute("Health", 0);
        SaveAttribute("Stamina", 0);
        SaveAttribute("Speed", 0);
        SaveAttribute("Damage", 0);
    }

    public void LoadSavedData()
    {
        _health = LoadAttribute("Health");
        _stamina = LoadAttribute("Stamina");
        _speed = LoadAttribute("Speed");
        _damage = LoadAttribute("Damage");
    }
}

public class PlayerStats : MonoBehaviour, IDamageHandler
{
    [SerializeField] private PlayerAttributes attributes;
    
    private int _experience = 0;
    public int Experience
    {
        get => _experience;
        private set => SaveAttribute("Experience", _experience = value);
    }
    
    private int _level = 0;
    public int Level
    {
        get => _level;
        private set => SaveAttribute("Level", _level = value);
    }
    
    public int Health = 100;
    public float InvincibilityTime = 1f;
    public int AttributePoints = 0;
    public int SkillPoints = 0;
    
    public int MaxHealth => 100 + 20 * attributes.Health;
    private float invincibilityTimer = 0;
    public bool IsInvincible => invincibilityTimer > 0;
    public bool IsDead => Health <= 0;
    public int XpRequired => 20 * Level + 100;

    void LoadSavedData()
    {
        _level = LoadAttribute("Level");
        _experience = LoadAttribute("Experience");
    }

    void Start()
    {
        LoadSavedData();
    }
    
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
    
    private static void SaveAttribute(string key, int value)
    {
        PlayerPrefs.SetInt($"Player_{key}", value);
        if (value != 0) SaveSystem.HasSave = true;
    }
    
    private static int LoadAttribute(string key, int defaultValue = default)
    {
        return PlayerPrefs.GetInt($"Player_{key}", defaultValue);
    }

    public static void ResetSavedData()
    {
        PlayerAttributes.ResetSavedAttributes();
        SaveAttribute("Experience", 0);
        SaveAttribute("Level", 0);
    }
    
    public void ModifyAttribute(string attributeName, int value)
    {
        if (AttributePoints <= 0) return;
        
        var field = typeof(PlayerAttributes).GetField(attributeName);
        if (field == null) throw new KeyNotFoundException();
        
        field.SetValue(attributes, (int)field.GetValue(attributes) + value);
        AttributePoints--;
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
