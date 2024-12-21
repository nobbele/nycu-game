using UnityEngine;
using TMPro;
using System;

public class InventoryItemUI : ItemUI
{
    [SerializeField] private GameObject countContainer;

    private TextMeshProUGUI countText;
    private InventoryItem inventoryItem;

    public new InventoryItem currentItem => inventoryItem;

    private void Awake()
    {
        base.Awake();
        if (countContainer != null)
        {
            countContainer.SetActive(false);
            countText = countContainer.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void Setup(InventoryItem item, Action<InventoryItem> clickCallback)
    {
        inventoryItem = item;
        base.Setup(item, item != null ? (_) => clickCallback?.Invoke(item) : null);
        UpdateCountDisplay();
    }

    private void UpdateCountDisplay()
    {
        if (countContainer != null)
        {
            bool showCount = inventoryItem != null && 
                            inventoryItem.type == ItemType.Normal && 
                            inventoryItem.count > 0;

            countContainer.SetActive(showCount);

            if (showCount && countText != null)
            {
                countText.text = inventoryItem.count.ToString();
            }
        }
    }

    public new void Clear()
    {
        inventoryItem = null;
        base.Clear();

        if (countContainer != null)
        {
            countContainer.SetActive(false);
        }
    }

    public void RefreshCount()
    {
        UpdateCountDisplay();
    }
}
