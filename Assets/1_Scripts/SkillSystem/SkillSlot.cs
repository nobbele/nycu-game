using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SkillSlotChangedEvent : UnityEvent<int, Skill> {}
public class SkillSlot : MonoBehaviour,  IDropHandler
{
    public Image icon;
    public SkillUI equippedSkillUI;
    public Skill equippedSkill;
    
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.TryGetComponent(out SkillUI skillUI))
        {
            if (!skillUI.IsSkillUnlocked())
            {
                Debug.Log($"Cannot equip locked skill: {skillUI.skill.skillName}");
                return;
            }

            EquipSkill(skillUI);
        }
    }

    public void EquipSkill(SkillUI skillUI)
    {
        if (skillUI.GetEquippedSlot() != null && skillUI.GetEquippedSlot() != this)
        {
            SwapSkill(skillUI.GetEquippedSlot());
        }
        else
        {
            if (equippedSkillUI != null)
            {
                equippedSkillUI.SetEquippedSlot(null);
            }
            
            icon.sprite = skillUI.skill.icon;
            equippedSkillUI = skillUI;
            equippedSkill = skillUI.skill;
            skillUI.SetEquippedSlot(this);
            equippedSkill.Reset();
            
            Debug.Log($"{equippedSkill.skillName} equipped");
        }
    }

    public void SwapSkill(SkillSlot skillSlot)
    {
        Sprite tempIcon = icon.sprite;
        SkillUI tempUI = equippedSkillUI;
        Skill tempSkill = equippedSkill;
        
        icon.sprite = skillSlot.icon.sprite;
        equippedSkillUI = skillSlot.equippedSkillUI;
        equippedSkill = skillSlot.equippedSkill;
        equippedSkillUI.SetEquippedSlot(this);
        equippedSkill.Reset();
        
        skillSlot.icon.sprite = tempIcon;
        skillSlot.equippedSkillUI = tempUI;
        skillSlot.equippedSkill = tempSkill;
        if (skillSlot.equippedSkillUI != null) skillSlot.equippedSkillUI.SetEquippedSlot(skillSlot);
        if (skillSlot.equippedSkill != null) skillSlot.equippedSkill.Reset();
        
        if (skillSlot.equippedSkill != null)
            Debug.Log($"{equippedSkill.skillName} and {skillSlot.equippedSkill.skillName} swapped");
        else
            Debug.Log($"{equippedSkill.skillName} and null swapped");
    }
    public void ClearSkill()
    {
        icon.sprite = null;
        if (equippedSkillUI != null)
        {
            equippedSkillUI.SetEquippedSlot(null);
        }
        equippedSkillUI = null;
        equippedSkill = null;
    }
}
