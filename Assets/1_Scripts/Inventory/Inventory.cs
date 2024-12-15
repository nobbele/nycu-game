using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    // Maximum slots in inventory
    [SerializeField] private int maxSlots = 20;
    
    // List of items in inventory
    private List<Item> items = new List<Item>();
    
    // Event triggered when inventory changes
    public UnityEvent OnInventoryChanged = new UnityEvent();

    // Add item to inventory
    public bool AddItem(Item item)
    {
        if (items.Count >= maxSlots)
            return false;
            
        items.Add(item);
        OnInventoryChanged.Invoke();
        return true;
    }

    // Remove item from inventory
    public bool RemoveItem(Item item)
    {
        bool removed = items.Remove(item);
        if (removed)
            OnInventoryChanged.Invoke();
        return removed;
    }

    // Get all items in inventory
    public List<Item> GetItems()
    {
        return new List<Item>(items);
    }
}
