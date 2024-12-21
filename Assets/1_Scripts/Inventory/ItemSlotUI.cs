using UnityEngine;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(ItemUI))]
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    private ItemUI itemUI;
    private Action<Item> onItemClicked;

    private void Awake()
    {
        itemUI = GetComponent<ItemUI>();
        if (itemUI == null)
        {
            Debug.LogError($"Missing ItemUI component on {gameObject.name}");
        }
    }

    public void Setup(Item item, Action<Item> clickCallback)
    {
        onItemClicked = clickCallback;
        itemUI?.Setup(item, null);
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
}
