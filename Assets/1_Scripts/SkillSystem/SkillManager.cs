using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SkillType
{
    Basic,
    Fire,
    Water,
    Grass
}

[Serializable]
public class GroupSkill
{
    public Skill skill;
    public bool locked = true;
}

[Serializable]
public class SkillGroup
{
    public SkillType type;
    public List<GroupSkill> skills = new List<GroupSkill>();
}

public class SkillUnlockedEvent : UnityEvent<SkillType, Skill> { }

public class SkillManager : MonoBehaviour
{
    [SerializeField] private List<SkillGroup> skillGroups = new List<SkillGroup>();
    public SkillUnlockedEvent onSkillUnlocked = new SkillUnlockedEvent();

    public bool TryUnlockSkill(string skillName)
    {
        if (string.IsNullOrEmpty(skillName)) return false;

        foreach (var group in skillGroups)
        {
            foreach (var groupSkill in group.skills)
            {
                if (groupSkill.skill != null && 
                    groupSkill.skill.name == skillName && 
                    groupSkill.locked)
                {
                    groupSkill.locked = false;
                    onSkillUnlocked.Invoke(group.type, groupSkill.skill);
                    return true;
                }
            }
        }
        return false;
    }

    public List<Skill> GetUnlockedSkills(SkillType type)
    {
        var unlockedSkills = new List<Skill>();
        
        foreach (var group in skillGroups)
        {
            if (group.type == type)
            {
                foreach (var groupSkill in group.skills)
                {
                    if (!groupSkill.locked && groupSkill.skill != null)
                    {
                        unlockedSkills.Add(groupSkill.skill);
                    }
                }
            }
        }
        return unlockedSkills;
    }

    public List<Skill> GetAllSkillsInGroup(SkillType type)
    {
        var skills = new List<Skill>();
        
        foreach (var group in skillGroups)
        {
            if (group.type == type)
            {
                foreach (var groupSkill in group.skills)
                {
                    if (groupSkill.skill != null)
                    {
                        skills.Add(groupSkill.skill);
                    }
                }
            }
        }
        return skills;
    }

    public bool IsSkillUnlocked(string skillName)
    {
        if (string.IsNullOrEmpty(skillName)) return false;

        foreach (var group in skillGroups)
        {
            foreach (var groupSkill in group.skills)
            {
                if (groupSkill.skill != null && 
                    groupSkill.skill.name == skillName)
                {
                    return !groupSkill.locked;
                }
            }
        }
        return false;
    }
}
