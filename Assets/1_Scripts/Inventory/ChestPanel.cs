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
            Debug.LogError("chestPanel: No backpack slots found!");
            return;
        }

        if (chestSlots == null || chestSlots.Length == 0)
        {
            Debug.LogError("chestPanel: No chest slots found!");
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
                Debug.LogError("chestPanel: Player inventory component not found!");
            }
        }
        else
        {
            Debug.LogError("chestPanel: Player instance not found!");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshUICoroutine());
    }

    public void ShowChestUI(ChestController chest)
    {
        if (chest == null) return;

        if (currentChest != null)
        {
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
        }

        currentChest = chest;
        currentChest.OnChestChanged.AddListener(RefreshChestInventory);
        
        // Disable interaction controller when inventory is shown
        if (player != null)
        {
            var interactionController = player.GetComponent<InteractionController>();
            if (interactionController != null)
            {
                interactionController.SetEnabled(false);
            }
        }
        
        gameObject.SetActive(true);
    }

    private IEnumerator RefreshUICoroutine()
    {
        if (isRefreshing) yield break;
        
        isRefreshing = true;
        
        // Wait for a frame to ensure all components are ready
        yield return null;
        
        RefreshPlayerInventory();
        
        if (currentChest != null)
        {
            // Wait for another frame to ensure chest items are initialized
            yield return null;
            RefreshChestInventory();
            
            // Double check after a short delay
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

        List<Item> items = currentChest.GetItems();
        
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

        List<Item> items = playerInventory.GetItems();
        for (int i = 0; i < items.Count && i < backpackSlots.Length; i++)
        {
            if (items[i] != null)
            {
                backpackSlots[i].Setup(items[i], OnPlayerItemClicked);
            }
        }
    }

    private void OnPlayerItemClicked(Item item)
    {
        if (currentChest != null && item != null)
        {
            playerInventory.RemoveItem(item);
            currentChest.AddItem(item);
        }
    }

    private void OnChestItemClicked(Item item)
    {
        if (item != null && playerInventory.AddItem(item))
        {
            currentChest.RemoveItem(item);
        }
    }

    public void HideUI()
    {
        StopAllCoroutines();
        
        if (currentChest != null)
        {
            // Play close animation before hiding UI
            currentChest.PlayCloseAnimation();
            
            currentChest.OnChestChanged.RemoveListener(RefreshChestInventory);
            currentChest = null;
            
            // Re-enable interaction controller when inventory is hidden
            if (player != null)
            {
                var interactionController = player.GetComponent<InteractionController>();
                if (interactionController != null)
                {
                    interactionController.SetEnabled(true);
                }
            }
        }
        
        gameObject.SetActive(false);
        ClearAllSlots();
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
