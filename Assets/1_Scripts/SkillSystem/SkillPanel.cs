using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillPointText;
    [SerializeField] private List<Skill> BasicSkill;
    [SerializeField] private List<SkillUI> BasicSkillUI;
    [SerializeField] private List<Skill> FireSkill;
    [SerializeField] private List<SkillUI> FireSkillUI;
    [SerializeField] private List<Skill> GrassSkill;
    [SerializeField] private List<SkillUI> GrassSkillUI;
    [SerializeField] private List<Skill> WaterSkill;
    [SerializeField] private List<SkillUI> WaterSkillUI;
    public List<SkillSlot> SkillSlots;
    private int skillPoints;

    public UnityEvent<int> onSkillPointsChanged = new();

    private void Start()
    {
        BasicSkillSetup();
        FireSkillSetup();
        GrassSkillSetup();
        WaterSkillSetup();
        SetAllEnabled();
    }

    private void BasicSkillSetup()
    {
        for (int i = 0; i < BasicSkillUI.Count; i++)
        {
            if (BasicSkill[i] != null) BasicSkillUI[i].SetSkill(BasicSkill[i]);
            BasicSkillUI[i].onSkillLevelChanged.AddListener(SetSkillPoint);
        }
    }

    private void FireSkillSetup()
    {
        for (int i = 0; i < FireSkillUI.Count; i++)
        {
            if (FireSkill[i] != null) FireSkillUI[i].SetSkill(FireSkill[i]);
            FireSkillUI[i].onSkillLevelChanged.AddListener(SetSkillPoint);
        }
    }

    private void GrassSkillSetup()
    {
        for (int i = 0; i < GrassSkillUI.Count; i++)
        {
            if (GrassSkill[i] != null) GrassSkillUI[i].SetSkill(GrassSkill[i]);
            GrassSkillUI[i].onSkillLevelChanged.AddListener(SetSkillPoint);
        }
    }

    private void WaterSkillSetup()
    {
        for (int i = 0; i < WaterSkillUI.Count; i++)
        {
            if (WaterSkill[i] != null) WaterSkillUI[i].SetSkill(WaterSkill[i]);
            WaterSkillUI[i].onSkillLevelChanged.AddListener(SetSkillPoint);
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
        for (int i = 0; i < BasicSkillUI.Count; i++)
        {
            BasicSkillUI[i].SetEnabled(skillPoints > 0);
        }
        for (int i = 0; i < FireSkillUI.Count; i++)
        {
            FireSkillUI[i].SetEnabled(skillPoints > 0);
        }
        for (int i = 0; i < GrassSkillUI.Count; i++)
        {
            GrassSkillUI[i].SetEnabled(skillPoints > 0);
        }
        for (int i = 0; i < WaterSkillUI.Count; i++)
        {
            WaterSkillUI[i].SetEnabled(skillPoints > 0);
        }
    }
}
