using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "RPG/Effect")]
public class Effect : ScriptableObject
{
    public string effectName;
    public string description;
    public Sprite icon;
    public float duration;
    public bool isStackable;
    public EffectType effectType; // Buff, Debuff, etc.
    public float magnitude; // Amount of stat change
    public GameObject particleEffect;
}

public enum EffectType { Buff, Debuff, DamageOverTime, HealOverTime, Instant }
