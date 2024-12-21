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
        public ItemType type = ItemType.Normal;
        public int count = 1;
        public int maxStack = 99;
    }

    [SerializeField] private List<InitialChestItem> initialItems = new List<InitialChestItem>();
    private List<InventoryItem> items = new List<InventoryItem>();
    
    private UnityEvent onChestChanged = new UnityEvent();
    public UnityEvent OnChestChanged => onChestChanged;
    
    [Header("Interaction Settings")]
    [SerializeField] private float maxInteractDistance = 3f;
    [SerializeField] private float maxInteractAngle = 45f;
    
    private Animator animator;
    private ChestPanel chestPanel;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeItems();
        animator = GetComponent<Animator>();
    }

    private void InitializeItems()
    {
        if (isInitialized) return;
        
        items.Clear();
        foreach (var initialItem in initialItems)
        {
            if (initialItem.icon != null)
            {
                items.Add(new InventoryItem(
                    System.Guid.NewGuid().ToString(),
                    initialItem.name,
                    initialItem.icon,
                    initialItem.type,
                    "",
                    initialItem.count,
                    initialItem.maxStack
                ));
            }
        }
        isInitialized = true;
        onChestChanged.Invoke();
    }

    public void OnInteract(GameObject player)
    {
        if (chestPanel == null && player != null)
        {
            var playerController = player.GetComponent<PlayerUIController>();
            if (playerController != null)
            {
                chestPanel = playerController.ChestPanel;
            }
        }

        if (chestPanel != null)
        {
            if (!isInitialized)
            {
                InitializeItems();
            }

            PlayOpenAnimation();
            chestPanel.ShowChestUI(this);
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

    public void AddItem(InventoryItem item)
    {
        if (item != null)
        {
            items.Add(item);
            onChestChanged.Invoke();
        }
    }

    public void RemoveItem(InventoryItem item)
    {
        if (items.Remove(item))
        {
            onChestChanged.Invoke();
        }
    }

    public List<InventoryItem> GetItems()
    {
        if (!isInitialized)
        {
            InitializeItems();
        }
        return items;
    }
}
