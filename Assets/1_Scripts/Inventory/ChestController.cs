using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestController : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class InitialChestItem
    {
        public string name;
        public Sprite icon;
    }

    [SerializeField] private List<InitialChestItem> initialItems = new List<InitialChestItem>();
    private List<Item> items = new List<Item>();
    
    private UnityEvent onChestChanged = new UnityEvent();
    public UnityEvent OnChestChanged => onChestChanged;
    
    [SerializeField] private float maxInteractDistance = 3f;
    [SerializeField] private float maxInteractAngle = 45f;
    private Animator animator;

    private InventoryUI inventoryUI;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeItems();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void InitializeItems()
    {
        if (isInitialized) return;
        
        items.Clear();
        foreach (var initialItem in initialItems)
        {
            if (initialItem.icon != null)
            {
                items.Add(new Item(
                    System.Guid.NewGuid().ToString(),
                    initialItem.name,
                    initialItem.icon
                ));
            }
        }
        isInitialized = true;
        
        onChestChanged.Invoke();
    }

    public void OnInteract(GameObject player)
    {
        if (inventoryUI == null && player != null)
        {
            var playerController = player.GetComponent<PlayerUIController>();
            if (playerController != null)
            {
                inventoryUI = playerController.InventoryUI;
            }
        }

        if (inventoryUI != null)
        {
            if (!isInitialized)
            {
                InitializeItems();
            }

            PlayOpenAnimation();
            inventoryUI.ShowChestUI(this);
        }
        else
        {
            Debug.LogWarning("Inventory UI reference not found!");
        }
    }

    public void PlayOpenAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }

    public void PlayCloseAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
    }

    public bool CanInteract(GameObject player)
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > maxInteractDistance) return false;

        Vector3 directionToChest = (transform.position - player.transform.position).normalized;
        float angle = Vector3.Angle(player.transform.forward, directionToChest);
        if (angle > maxInteractAngle) return false;

        return true;
    }

    public string GetInteractPrompt()
    {
        return "Press E to open";
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        onChestChanged.Invoke();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        onChestChanged.Invoke();
    }

    public List<Item> GetItems()
    {
        if (!isInitialized)
        {
            InitializeItems();
        }
        return items;
    }
}
