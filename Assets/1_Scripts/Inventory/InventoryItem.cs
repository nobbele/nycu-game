using UnityEngine;

public enum ItemType
{
    Normal,
    Skill
}

public class InventoryItem : Item
{
    public ItemType type { get; private set; }
    public int count { get; private set; }
    public int maxStack { get; private set; }

    public InventoryItem(string id, string name, Sprite icon, ItemType type, string info = "", int count = 1, int maxStack = 99) 
        : base(id, name, icon, info)
    {
        this.type = type;
        this.count = count;
        this.maxStack = maxStack;
    }

    public bool CanStack(InventoryItem other)
    {
        if (type != ItemType.Normal || other.type != ItemType.Normal)
            return false;
            
        return id == other.id && count < maxStack;
    }

    public bool TryStack(InventoryItem other)
    {
        if (!CanStack(other))
            return false;

        int spaceLeft = maxStack - count;
        int amountToAdd = Mathf.Min(spaceLeft, other.count);
        
        if (amountToAdd <= 0)
            return false;

        count += amountToAdd;
        other.count -= amountToAdd;
        
        return true;
    }

    public InventoryItem Split(int amount)
    {
        if (amount <= 0 || amount >= count)
            return null;

        count -= amount;
        return new InventoryItem(id, name, icon, type, info, amount, maxStack);
    }

    public void SetCount(int newCount)
    {
        if (type == ItemType.Normal)
        {
            count = Mathf.Clamp(newCount, 0, maxStack);
        }
    }
}
