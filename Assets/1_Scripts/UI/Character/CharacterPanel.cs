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
    
    [Header("Skill Containers")]
    [SerializeField] private Transform basicSkillContainer;
    [SerializeField] private Transform fireSkillContainer;
    [SerializeField] private Transform grassSkillContainer;
    [SerializeField] private Transform waterSkillContainer;

    [Header("Backpack")]
    [SerializeField] private Transform backpackContainer;
    
    [Header("Skill Slots")]
    public List<SkillSlot> SkillSlots;

    private Dictionary<SkillType, List<SkillUI>> skillUIGroups = new Dictionary<SkillType, List<SkillUI>>();
    private SkillManager skillManager;
    private Inventory playerInventory;
    private int skillPoints;
    private ItemSlotUI[] backpackSlots;

    private void Awake()
    {
        gameObject.SetActive(false);
        InitializeSkillUIGroups();
        InitializeBackpack();
    }

    private void Start()
    {
        if (Player.Instance == null) return;
        
        InitializePlayerComponents();
        SetupSkillManager();
        SetupInventory();
    }

    private void InitializeSkillUIGroups()
    {
        skillUIGroups[SkillType.Basic] = new List<SkillUI>(basicSkillContainer.GetComponentsInChildren<SkillUI>(true));
        skillUIGroups[SkillType.Fire] = new List<SkillUI>(fireSkillContainer.GetComponentsInChildren<SkillUI>(true));
        skillUIGroups[SkillType.Grass] = new List<SkillUI>(grassSkillContainer.GetComponentsInChildren<SkillUI>(true));
        skillUIGroups[SkillType.Water] = new List<SkillUI>(waterSkillContainer.GetComponentsInChildren<SkillUI>(true));
    }

    private void InitializeBackpack()
    {
        if (backpackContainer != null)
        {
            backpackSlots = backpackContainer.GetComponentsInChildren<ItemSlotUI>(true);
        }
    }

    private void InitializePlayerComponents()
    {
        skillManager = Player.Instance.GetComponent<SkillManager>();
        playerInventory = Player.Instance.GetComponent<Inventory>();
    }

    private void SetupSkillManager()
    {
        if (skillManager == null) return;

        skillManager.onSkillUnlocked.AddListener(OnSkillUnlocked);
        SetupSkillUIs();
    }

    private void SetupInventory()
    {
        if (playerInventory == null) return;
        
        playerInventory.OnInventoryChanged.AddListener(RefreshInventoryItems);
    }

    private void SetupSkillUIs()
    {
        foreach (var group in skillUIGroups)
        {
            List<Skill> allSkills = skillManager.GetAllSkillsInGroup(group.Key);
            List<Skill> unlockedSkills = skillManager.GetUnlockedSkills(group.Key);
            
            for (int i = 0; i < group.Value.Count && i < allSkills.Count; i++)
            {
                SkillUI skillUI = group.Value[i];
                skillUI.onSkillLevelChanged.AddListener(SetSkillPoint);
                
                if (unlockedSkills.Contains(allSkills[i]))
                {
                    skillUI.SetSkill(allSkills[i]);
                }
                else
                {
                    skillUI.SetSkill(null);
                }
            }
        }
    }

    private void OnEnable()
    {
        RefreshAllSkillUIs();
        RefreshInventoryItems();
    }

    private void RefreshAllSkillUIs()
    {
        if (skillManager == null) return;

        foreach (var group in skillUIGroups)
        {
            List<Skill> unlockedSkills = skillManager.GetUnlockedSkills(group.Key);
            foreach (var skillUI in group.Value)
            {
                if (skillUI.CurrentSkill != null && !unlockedSkills.Contains(skillUI.CurrentSkill))
                {
                    skillUI.SetSkill(null);
                }
            }
        }
    }

    private void OnSkillUnlocked(SkillType type, Skill skill)
    {
        if (!skillUIGroups.TryGetValue(type, out var skillUIs)) return;
        
        foreach (var skillUI in skillUIs)
        {
            // If this UI slot is empty or matches the unlocked skill
            if (skillUI.CurrentSkill == null || skillUI.CurrentSkill.name == skill.name)
            {
                skillUI.SetSkill(skill);
                break;
            }
        }
    }

    private void RefreshInventoryItems()
    {
        if (backpackSlots == null || playerInventory == null) return;

        foreach (var slot in backpackSlots)
        {
            slot.Clear();
        }

        var items = playerInventory.GetItems();
        for (int i = 0; i < items.Count && i < backpackSlots.Length; i++)
        {
            backpackSlots[i].Setup(items[i], null);
        }
    }

    public void SetSkillPoint(int points)
    {
        skillPoints += points;
        skillPointText.text = $"Skill Points:{skillPoints}";
        SetAllSkillUIEnabled();
    }

    private void SetAllSkillUIEnabled()
    {
        foreach (var group in skillUIGroups.Values)
        {
            foreach (var skillUI in group)
            {
                skillUI.SetEnabled(skillPoints > 0);
            }
        }
    }

    private void OnDestroy()
    {
        if (skillManager != null)
        {
            skillManager.onSkillUnlocked.RemoveListener(OnSkillUnlocked);
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged.RemoveListener(RefreshInventoryItems);
        }
    }
}
