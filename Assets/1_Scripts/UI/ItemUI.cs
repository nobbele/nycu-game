using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI info;
    [SerializeField] protected GameObject hoverContainer;
    protected Image hoverMask;
    protected TextMeshProUGUI hoverName;
    
    public Item currentItem { get; private set; }
    protected Action<Item> onItemClicked;
    protected Sprite defaultIcon;
    
    protected void Awake()
    {
        if (icon != null)
        {
            defaultIcon = icon.sprite;
        }
        
        if (info != null)
        {
            info.gameObject.SetActive(true);
        }
        
        if (hoverContainer != null)
        {
            hoverContainer.SetActive(false);
            hoverMask = hoverContainer.GetComponent<Image>();
            hoverName = hoverContainer.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        Clear();
    }

    public void Setup(Item item, Action<Item> clickCallback)
    {
        currentItem = item;
        onItemClicked = clickCallback;

        if (item == null)
        {
            Clear();
            return;
        }

        if (icon != null)
        {
            icon.sprite = item.icon == null ? defaultIcon : item.icon;
            icon.gameObject.SetActive(true);
        }

        if (hoverName != null)
        {
            hoverName.text = item.name;
        }

        if (info != null)
        {
            info.text = item.info ?? "";
            info.gameObject.SetActive(true);
        }
    }

    public void SetInfo(string infoText)
    {
        if (info != null)
        {
            info.text = infoText ?? "";
            info.gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        currentItem = null;
        onItemClicked = null;

        if (icon != null)
        {
            icon.sprite = defaultIcon;
            icon.gameObject.SetActive(true);
        }
        
        if (hoverName != null)
        {
            hoverName.text = "";
        }

        if (info != null)
        {
            info.text = "";
            info.gameObject.SetActive(true);
        }

        if (hoverContainer != null)
        {
            hoverContainer.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverContainer != null && currentItem != null)
        {
            hoverContainer.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverContainer != null)
        {
            hoverContainer.SetActive(false);
        }
    }

    public void RefreshInfo()
    {
        if (info != null && currentItem != null)
        {
            info.text = currentItem.info;
            info.gameObject.SetActive(true);
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
