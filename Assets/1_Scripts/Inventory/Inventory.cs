using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 12;
    private List<InventoryItem> items = new List<InventoryItem>();
    
    public UnityEvent OnInventoryChanged = new UnityEvent();
    
    public bool AddItem(InventoryItem newItem)
    {
        if (newItem == null) return false;

        // Handle skill items
        if (newItem.type == ItemType.Skill)
        {
            var skillManager = GetComponent<SkillManager>();
            if (skillManager != null)
            {
                bool unlocked = skillManager.TryUnlockSkill(newItem.id);
                if (unlocked)
                {
                    OnInventoryChanged.Invoke();
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("Inventory: SkillManager not found when trying to unlock skill");
            }
            return false;
        }

        // Try stacking with existing items
        foreach (var item in items)
        {
            if (item.TryStack(newItem))
            {
                if (newItem.count <= 0)
                {
                    OnInventoryChanged.Invoke();
                    return true;
                }
            }
        }

        // Add as new item if couldn't fully stack
        if (newItem.count > 0 && items.Count < maxSlots)
        {
            items.Add(newItem);
            OnInventoryChanged.Invoke();
            return true;
        }

        return false;
    }

    public bool RemoveItem(InventoryItem item)
    {
        if (item == null) return false;

        bool removed = items.Remove(item);
        if (removed)
        {
            OnInventoryChanged.Invoke();
        }
        return removed;
    }

    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>(items);
    }
}
