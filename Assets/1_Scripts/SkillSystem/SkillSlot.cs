using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SkillChangeEvent : UnityEvent<Skill> {}

public class SkillSlot : MonoBehaviour, IDropHandler
{
    public Image icon;
    public SkillUI equippedSkillUI;
    public Skill equippedSkill;
    
    [SerializeField] private SkillChangeEvent _onSkillChanged = new();
    public SkillChangeEvent onSkillChanged => _onSkillChanged;
    
    void Awake()
    {
        if (_onSkillChanged == null)
            _onSkillChanged = new();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && droppedObject.TryGetComponent(out SkillUI skillUI))
        {
            if (!skillUI.IsSkillUnlocked())
            {
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
            
            _onSkillChanged.Invoke(equippedSkill);
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
        _onSkillChanged.Invoke(equippedSkill);
        
        skillSlot.icon.sprite = tempIcon;
        skillSlot.equippedSkillUI = tempUI;
        skillSlot.equippedSkill = tempSkill;
        if (skillSlot.equippedSkillUI != null) skillSlot.equippedSkillUI.SetEquippedSlot(skillSlot);
        if (skillSlot.equippedSkill != null) 
        {
            skillSlot.equippedSkill.Reset();
            skillSlot._onSkillChanged.Invoke(skillSlot.equippedSkill);
        }
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
        _onSkillChanged.Invoke(null);
    }
}
