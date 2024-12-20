using UnityEngine;

public class SkillBar : MonoBehaviour
{
    [SerializeField] private string[] keyBindings = { "1", "2", "3", "4" };
    
    private SkillBarSlot[] skillBarSlots;
    private CharacterPanel characterPanel;

    private void Start()
    {
        InitializeReferences();
        SetupSkillSynchronization();
    }

    private void InitializeReferences()
    {
        var player = Player.Instance;
        if (!player)
        {
            Debug.LogError("SkillBarUI: Player instance not found!");
            return;
        }

        characterPanel = player.GetComponent<PlayerUIController>()?.CharacterPanel;
        if (!characterPanel)
        {
            Debug.LogError("SkillBarUI: CharacterPanel not found!");
            return;
        }

        skillBarSlots = GetComponentsInChildren<SkillBarSlot>();
        if (skillBarSlots.Length == 0)
        {
            Debug.LogError("SkillBarUI: No skill slots found in children!");
            return;
        }

        for (int i = 0; i < skillBarSlots.Length; i++)
        {
            if (i < keyBindings.Length)
            {
                skillBarSlots[i].SetKeyText(keyBindings[i]);
            }
        }
    }

    private void SetupSkillSynchronization()
    {
        if (!characterPanel || skillBarSlots == null) return;

        int slotCount = Mathf.Min(characterPanel.SkillSlots.Count, skillBarSlots.Length);

        for (int i = 0; i < slotCount; i++)
        {
            int index = i;
            var panelSlot = characterPanel.SkillSlots[index];
            var barSlot = skillBarSlots[index];

            panelSlot.onSkillChanged.AddListener((skill) =>
            {
                barSlot.UpdateSkill(skill);
            });

            if (panelSlot.equippedSkill != null)
            {
                barSlot.UpdateSkill(panelSlot.equippedSkill);
            }
        }
    }
}