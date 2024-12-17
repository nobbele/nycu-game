using System;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public float cooldown;
    private float lastUsedTime = -999f;
    public float LastUsedTime => lastUsedTime;
    
    public bool CanCast()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    public void Cast(GameObject caster)
    {
        if (CanCast())
        {
            Execute(caster);
            lastUsedTime = Time.time;
        }
    }

    public void Reset()
    {
        lastUsedTime = -999f;
    }

    protected abstract void Execute(GameObject caster);
}
