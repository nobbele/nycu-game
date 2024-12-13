using System;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public float cooldown; // Time before the skill can be used again
    private float lastUsedTime;
    
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
        else
        {
            Debug.Log($"Skill {skillName} is on cooldown! {Time.time}, {lastUsedTime}");
        }
    }

    public void Reset()
    {
        lastUsedTime = 0f;
    }

    protected abstract void Execute(GameObject caster);
}