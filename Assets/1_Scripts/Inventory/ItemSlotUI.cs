using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Image iconImage;
    [SerializeField] public TextMeshProUGUI nameText;

    private Item currentItem;
    private Action<Item> onItemClicked;

    public void Setup(Item item, Action<Item> clickCallback)
    {
        currentItem = item;
        onItemClicked = clickCallback;

        if (item == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.gameObject.SetActive(true);
        }

        if (nameText != null)
        {
            nameText.text = item.name;
            nameText.gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        currentItem = null;
        onItemClicked = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (nameText != null)
        {
            nameText.text = "";
            nameText.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null && onItemClicked != null)
        {
            onItemClicked.Invoke(currentItem);
        }
    }
}
