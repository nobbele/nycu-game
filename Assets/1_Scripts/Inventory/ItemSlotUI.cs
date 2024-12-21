using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(InventoryItemUI))]
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    private InventoryItemUI itemUI;
    private Action<InventoryItem> onItemClicked;
    
    private void Awake()
    {
        itemUI = GetComponent<InventoryItemUI>();
        if (itemUI == null)
        {
            Debug.LogError($"Missing InventoryItemUI component on {gameObject.name}");
        }
    }

    public void Setup(InventoryItem item, Action<InventoryItem> clickCallback)
    {
        onItemClicked = clickCallback;
        itemUI?.Setup(item, clickCallback);
    }

    public void Clear()
    {
        onItemClicked = null;
        itemUI?.Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemUI?.currentItem != null && onItemClicked != null)
        {
            onItemClicked.Invoke(itemUI.currentItem);
        }
    }
    
    public void RefreshCount()
    {
        itemUI?.RefreshCount();
    }
}
