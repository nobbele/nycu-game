using UnityEngine;

public class SkillBar : MonoBehaviour
{
    [SerializeField] private string[] keyBindings = { "1", "2", "3", "4" };
    
    private SkillBarSlot[] skillBarSlots;
    private SkillPanel skillPanel;

    private void Start()
    {
        InitializeReferences();
        SetupSkillSynchronization();
    }

    private void InitializeReferences()
    {
        // Get Player and required components
        var player = Player.Instance;
        if (!player)
        {
            Debug.LogError("SkillBarUI: Player instance not found!");
            return;
        }

        // Get SkillPanel reference
        skillPanel = player.GetComponent<PlayerUIController>()?.SkillPanel;
        if (!skillPanel)
        {
            Debug.LogError("SkillBarUI: SkillPanel not found!");
            return;
        }

        // Get local skill bar slots
        skillBarSlots = GetComponentsInChildren<SkillBarSlot>();
        if (skillBarSlots.Length == 0)
        {
            Debug.LogError("SkillBarUI: No skill slots found in children!");
            return;
        }

        // Initialize key bindings
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
        if (!skillPanel || skillBarSlots == null) return;

        // Sync the number of slots available
        int slotCount = Mathf.Min(skillPanel.SkillSlots.Count, skillBarSlots.Length);

        // Set up synchronization for each slot
        for (int i = 0; i < slotCount; i++)
        {
            int index = i;
            var panelSlot = skillPanel.SkillSlots[index];
            var barSlot = skillBarSlots[index];

            // Sync from panel to bar
            panelSlot.onSkillChanged.AddListener((skill) =>
            {
                barSlot.UpdateSkill(skill);
            });

            // Initial sync if panel slot has a skill
            if (panelSlot.equippedSkill != null)
            {
                barSlot.UpdateSkill(panelSlot.equippedSkill);
            }
        }
    }
}
