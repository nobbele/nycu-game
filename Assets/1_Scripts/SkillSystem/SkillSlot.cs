using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SkillChangeEvent : UnityEvent<Skill> { }

public class SkillSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image icon;
    public SkillUI equippedSkillUI;
    public Skill equippedSkill;
    
    [SerializeField] private SkillChangeEvent _onSkillChanged = new();
    public SkillChangeEvent onSkillChanged => _onSkillChanged;
    
    private ItemUI itemUI;
    private string defaultInfo;
    
    void Awake()
    {
        if (_onSkillChanged == null)
            _onSkillChanged = new();
            
        itemUI = GetComponent<ItemUI>();

        var infoText = transform.Find("Info")?.GetComponent<TextMeshProUGUI>();
        if (infoText != null)
        {
            defaultInfo = infoText.text;
        }
    }
    
    void Start()
    {
        var infoText = transform.Find("Info")?.GetComponent<TextMeshProUGUI>();
        if (infoText != null)
        {
            infoText.text = defaultInfo;
        }
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
            
            // Update icon and ItemUI
            if (icon != null)
            {
                icon.sprite = skillUI.CurrentSkill.icon;
                icon.gameObject.SetActive(true);
            }
            if (itemUI != null)
            {
                Item skillItem = new Item(
                    skillUI.CurrentSkill.name,
                    skillUI.CurrentSkill.skillName,
                    skillUI.CurrentSkill.icon,
                    defaultInfo // Use the default info text
                );
                itemUI.Setup(skillItem, null);
            }
            
            equippedSkillUI = skillUI;
            equippedSkill = skillUI.CurrentSkill;
            skillUI.SetEquippedSlot(this);
            equippedSkill.Reset();
            
            _onSkillChanged.Invoke(equippedSkill);
        }
    }

    public void SwapSkill(SkillSlot skillSlot)
    {
        // Store temporary references
        Sprite tempIcon = icon.sprite;
        SkillUI tempUI = equippedSkillUI;
        Skill tempSkill = equippedSkill;
        
        // Update current slot
        icon.sprite = skillSlot.icon.sprite;
        equippedSkillUI = skillSlot.equippedSkillUI;
        equippedSkill = skillSlot.equippedSkill;
        if (itemUI != null)
        {
            Item newItem = new Item(
                equippedSkill.name,
                equippedSkill.skillName,
                equippedSkill.icon,
                defaultInfo // Use the default info text
            );
            itemUI.Setup(newItem, null);
        }
        equippedSkillUI.SetEquippedSlot(this);
        equippedSkill.Reset();
        _onSkillChanged.Invoke(equippedSkill);
        
        // Update other slot
        skillSlot.icon.sprite = tempIcon;
        skillSlot.equippedSkillUI = tempUI;
        skillSlot.equippedSkill = tempSkill;
        if (skillSlot.itemUI != null)
        {
            Item tempItem = new Item(
                tempSkill.name,
                tempSkill.skillName,
                tempSkill.icon,
                skillSlot.defaultInfo // Use the other slot's default info text
            );
            skillSlot.itemUI.Setup(tempItem, null);
        }
        if (skillSlot.equippedSkillUI != null) 
        {
            skillSlot.equippedSkillUI.SetEquippedSlot(skillSlot);
        }
        if (skillSlot.equippedSkill != null) 
        {
            skillSlot.equippedSkill.Reset();
            skillSlot._onSkillChanged.Invoke(skillSlot.equippedSkill);
        }
    }

    public void ClearSkill()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(true);
        }
        if (itemUI != null)
        {
            Item emptyItem = new Item("", "", null, defaultInfo);
            itemUI.Setup(emptyItem, null);
        }
        if (equippedSkillUI != null)
        {
            equippedSkillUI.SetEquippedSlot(null);
        }
        equippedSkillUI = null;
        equippedSkill = null;
        _onSkillChanged.Invoke(null);
    }
}
