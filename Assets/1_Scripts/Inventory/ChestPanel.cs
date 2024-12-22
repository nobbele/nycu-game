using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChestPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform backpackItemContainer;
    [SerializeField] private Transform chestItemContainer;
    
    private Player player;
    private Inventory playerInventory;
    private ChestController currentChest;
    private ItemSlotUI[] backpackSlots;
    private ItemSlotUI[] chestSlots;
    private PlayerUIController playerUIController;

    private void Awake()
    {
        backpackSlots = backpackItemContainer.GetComponentsInChildren<ItemSlotUI>(true);
        chestSlots = chestItemContainer.GetComponentsInChildren<ItemSlotUI>(true);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        ValidateComponents();
        SetupCloseButton();
        RefreshUI();
    }

    private void ValidateComponents()
    {
        if (backpackSlots == null || backpackSlots.Length == 0)
        {
            Debug.LogError("ChestPanel: No backpack slots found!");
            return;
        }

        if (chestSlots == null || chestSlots.Length == 0)
        {
            Debug.LogError("ChestPanel: No chest slots found!");
            return;
        }

        player = Player.Instance;
        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                playerInventory.OnInventoryChanged.AddListener(RefreshPlayerInventory);
                ClearAllSlots();
            }
            else
            {
                Debug.LogError("ChestPanel: Player inventory component not found!");
            }
        }
        else
        {
            Debug.LogError("ChestPanel: Player instance not found!");
        }
    }

    private void SetupCloseButton()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
    }

    public void Initialize(PlayerUIController controller)
    {
        playerUIController = controller;
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void ShowChestUI(ChestController chest)
    {
        if (chest == null || playerUIController == null) return;

        UnsubscribeFromCurrentChest();
        
        currentChest = chest;
        RefreshUI();
        currentChest.OnChestChanged.AddListener(RefreshChestInventory);
        
        playerUIController.ShowUI(gameObject);
    }

    private void UnsubscribeFromCurrentChest()
    {
        if (currentChest != null)
        {
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
            currentChest = null;
        }
    }

    private void HideUI()
    {
        if (currentChest != null)
        {
            currentChest.PlayCloseAnimation();
            UnsubscribeFromCurrentChest();
        }
        
        if (playerUIController != null)
        {
            playerUIController.HideUI(gameObject);
        }
        
        ClearAllSlots();
    }

    private void RefreshUI()
    {
        RefreshPlayerInventory();
        RefreshChestInventory();
    }

    private void RefreshChestInventory()
    {
        if (currentChest == null) 
        {
            ClearChestSlots();
            return;
        }

        List<InventoryItem> items = currentChest.GetItems();
        UpdateChestSlots(items);
    }

    private void ClearChestSlots()
    {
        foreach (var slot in chestSlots)
        {
            slot.Clear();
        }
    }

    private void UpdateChestSlots(List<InventoryItem> items)
    {
        ClearChestSlots();
        
        for (int i = 0; i < items.Count && i < chestSlots.Length; i++)
        {
            if (items[i] != null)
            {
                chestSlots[i].Setup(items[i], OnChestItemClicked);
            }
        }
    }

    private void RefreshPlayerInventory()
    {
        if (playerInventory == null) return;

        List<InventoryItem> items = playerInventory.GetItems();
        UpdatePlayerSlots(items);
    }

    private void UpdatePlayerSlots(List<InventoryItem> items)
    {
        foreach (var slot in backpackSlots)
        {
            slot.Clear();
        }

        for (int i = 0; i < items.Count && i < backpackSlots.Length; i++)
        {
            if (items[i] != null)
            {
                backpackSlots[i].Setup(items[i], OnPlayerItemClicked);
            }
        }
    }

    private void OnPlayerItemClicked(InventoryItem item)
    {
        if (currentChest != null && item != null && item.type == ItemType.Normal)
        {
            playerInventory.RemoveItem(item);
            currentChest.AddItem(item);
        }
    }

    private void OnChestItemClicked(InventoryItem item)
    {
        if (item == null) return;

        if (item.type == ItemType.Skill)
        {
            HandleSkillItem(item);
        }
        else if (item.type == ItemType.Normal)
        {
            HandleNormalItem(item);
        }
    }

    private void HandleSkillItem(InventoryItem item)
    {
        var skillManager = player.GetComponent<SkillManager>();
        if (skillManager != null && skillManager.TryUnlockSkill(item.name))
        {
            if (playerUIController != null)
            {
                playerUIController.ShowMessage($"Unlock {item.name}");
            }
            
            currentChest.RemoveItem(item);
        }
    }

    private void HandleNormalItem(InventoryItem item)
    {
        if (playerInventory != null)
        {
            currentChest.RemoveItem(item);
            playerInventory.AddItem(item);
        }
    }

    private void ClearAllSlots()
    {
        foreach (var slot in backpackSlots)
        {
            slot.Clear();
        }
        foreach (var slot in chestSlots)
        {
            slot.Clear();
        }
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged.RemoveListener(RefreshPlayerInventory);
        }

        UnsubscribeFromCurrentChest();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideUI);
        }
    }
}
