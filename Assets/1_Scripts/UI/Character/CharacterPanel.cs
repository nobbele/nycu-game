using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CharacterPanel : MonoBehaviour
{
    [Header("Character Attributes")]
    public AttributeUI DamageAttributeUI;
    public AttributeUI HealthAttributeUI;
    public TMP_Text AttributePointsLeftText;

    [Header("Skill Points")]
    [SerializeField] private TextMeshProUGUI skillPointText;
    
    [Header("Basic Skills")]
    [SerializeField] private List<Skill> BasicSkill;
    [SerializeField] private List<SkillUI> BasicSkillUI;
    
    [Header("Fire Skills")]
    [SerializeField] private List<Skill> FireSkill;
    [SerializeField] private List<SkillUI> FireSkillUI;
    
    [Header("Grass Skills")]
    [SerializeField] private List<Skill> GrassSkill;
    [SerializeField] private List<SkillUI> GrassSkillUI;
    
    [Header("Water Skills")]
    [SerializeField] private List<Skill> WaterSkill;
    [SerializeField] private List<SkillUI> WaterSkillUI;
    
    [Header("Skill Slots")]
    public List<SkillSlot> SkillSlots;

    private int skillPoints;
    private List<ItemUI> BasicItemUI;
    private List<ItemUI> FireItemUI;
    private List<ItemUI> GrassItemUI;
    private List<ItemUI> WaterItemUI;

    public UnityEvent<int> onSkillPointsChanged = new();

    private void Awake()
    {
        gameObject.SetActive(false);
        InitializeItemUIs();
    }

    private void Start()
    {
        SetupAllSkills();
        SetAllEnabled();
    }

    private void InitializeItemUIs()
    {
        BasicItemUI = InitializeItemUIList(BasicSkillUI);
        FireItemUI = InitializeItemUIList(FireSkillUI);
        GrassItemUI = InitializeItemUIList(GrassSkillUI);
        WaterItemUI = InitializeItemUIList(WaterSkillUI);
    }

    private List<ItemUI> InitializeItemUIList(List<SkillUI> skillUIs)
    {
        var itemUIs = new List<ItemUI>();
        foreach (var skillUI in skillUIs)
        {
            itemUIs.Add(skillUI.GetComponent<ItemUI>());
        }
        return itemUIs;
    }

    private void SetupAllSkills()
    {
        SetupSkillGroup(BasicSkill, BasicSkillUI, BasicItemUI);
        SetupSkillGroup(FireSkill, FireSkillUI, FireItemUI);
        SetupSkillGroup(GrassSkill, GrassSkillUI, GrassItemUI);
        SetupSkillGroup(WaterSkill, WaterSkillUI, WaterItemUI);
    }

    private void SetupSkillGroup(List<Skill> skills, List<SkillUI> skillUIs, List<ItemUI> itemUIs)
    {
        for (int i = 0; i < skillUIs.Count; i++)
        {
            if (skills[i] != null)
            {
                SetupSkillPair(skills[i], skillUIs[i], itemUIs[i]);
            }
        }
    }

    private void SetupSkillPair(Skill skill, SkillUI skillUI, ItemUI itemUI)
    {
        skillUI.SetSkill(skill);
        skillUI.onSkillLevelChanged.AddListener(SetSkillPoint);
        skillUI.onSkillLevelChanged.AddListener((i) => UpdateItemInfo(skillUI, itemUI));

        Item skillItem = new Item(
            skill.name,
            skill.skillName,
            skill.icon,
            "LV:0"
        );

        itemUI.Setup(skillItem, null);
    }

    private void UpdateItemInfo(SkillUI skillUI, ItemUI itemUI)
    {
        if (itemUI != null && itemUI.currentItem != null)
        {
            itemUI.currentItem.info = $"LV:{skillUI.CurrentLevel}";
            itemUI.RefreshInfo();
        }
    }

    public void SetSkillPoint(int i)
    {
        skillPoints += i;
        skillPointText.text = $"Skill Points:{skillPoints}";
        onSkillPointsChanged.Invoke(skillPoints);
        SetAllEnabled();
    }

    private void SetAllEnabled()
    {
        SetSkillGroupEnabled(BasicSkillUI);
        SetSkillGroupEnabled(FireSkillUI);
        SetSkillGroupEnabled(GrassSkillUI);
        SetSkillGroupEnabled(WaterSkillUI);
    }

    private void SetSkillGroupEnabled(List<SkillUI> skillUIs)
    {
        foreach (var skillUI in skillUIs)
        {
            skillUI.SetEnabled(skillPoints > 0);
        }
    }
}
