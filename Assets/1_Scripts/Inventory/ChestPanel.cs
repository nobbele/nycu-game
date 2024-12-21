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
    private bool isRefreshing = false;
    private PlayerUIController playerUIController;

    private void Awake()
    {
        backpackSlots = backpackItemContainer.GetComponentsInChildren<ItemSlotUI>(true);
        chestSlots = chestItemContainer.GetComponentsInChildren<ItemSlotUI>(true);
        gameObject.SetActive(false);
    }

    private void Start()
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
        StartCoroutine(RefreshUICoroutine());
    }

    public void ShowChestUI(ChestController chest)
    {
        if (chest == null || playerUIController == null) return;

        if (currentChest != null)
        {
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
        }

        currentChest = chest;
        currentChest.OnChestChanged.AddListener(RefreshChestInventory);
        playerUIController.ShowUI(gameObject);
    }

    private void HideUI()
    {
        StopAllCoroutines();
        
        if (currentChest != null)
        {
            currentChest.PlayCloseAnimation();
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
            currentChest = null;
        }
        
        if (playerUIController != null)
        {
            playerUIController.HideUI(gameObject);
        }
        
        ClearAllSlots();
    }

    private IEnumerator RefreshUICoroutine()
    {
        if (isRefreshing) yield break;
        
        isRefreshing = true;
        yield return null;
        
        RefreshPlayerInventory();
        
        if (currentChest != null)
        {
            yield return null;
            RefreshChestInventory();
            
            yield return new WaitForSeconds(0.1f);
            if (currentChest != null)
            {
                RefreshChestInventory();
            }
        }
        
        isRefreshing = false;
    }

    private void RefreshChestInventory()
    {
        if (currentChest == null) return;

        foreach (var slot in chestSlots)
        {
            slot.Clear();
        }

        List<InventoryItem> items = currentChest.GetItems();
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

        foreach (var slot in backpackSlots)
        {
            slot.Clear();
        }

        List<InventoryItem> items = playerInventory.GetItems();
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
            var skillManager = player.GetComponent<SkillManager>();
            if (skillManager != null)
            {
                if (skillManager.TryUnlockSkill(item.name))
                {
                    if (playerUIController != null)
                    {
                        playerUIController.ShowMessage($"Unlock {item.name}");
                    }
                    
                    currentChest.RemoveItem(item);
                    RefreshChestInventory();
                }
            }
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

        if (currentChest != null)
        {
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideUI);
        }
    }
}
