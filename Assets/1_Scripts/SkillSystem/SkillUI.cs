using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // UI Components
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private Image icon;
    [SerializeField] private Button increment;
    [SerializeField] private Button decrement;
    [SerializeField] private GameObject blocker;

    public UnityEvent<int> onSkillLevelChanged = new();
    
    public Skill CurrentSkill => skill;
    private Skill skill;
    private SkillSlot equippedSlot;
    private Image draggableIcon;
    private Canvas targetCanvas;
    private int lvl = 0;
    public int CurrentLevel => lvl;
    private bool skillUnlocked = false;
    
    private void Awake()
    {
        // Find the parent canvas for drag operations
        targetCanvas = GetComponentInParent<Canvas>();
        if (targetCanvas == null)
        {
            Debug.LogError("SkillUI: Cannot find parent Canvas!");
        }

        // Initialize UI state and bind button events
        increment.onClick.AddListener(AddLvl);
        decrement.onClick.AddListener(ReduceLvl);
        
        nameText.text = "";
        lvText.text = "LV:0";
        icon.sprite = null;
        UpdateButtonStates(false);
        SetSkillLockState();
    }

    private void Start()
    {
        lvText.text = "LV:0";
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        if (skill != null)
        {
            nameText.text = skill.skillName;
            icon.sprite = skill.icon;
        }
        else
        {
            nameText.text = "";
            icon.sprite = null;
        }
        UpdateButtonStates(true);
    }

    public void AddLvl()
    {
        lvText.text = $"LV:{++lvl}";
        onSkillLevelChanged.Invoke(-1);
        SetSkillLockState();
    }

    public void ReduceLvl()
    {
        lvText.text = $"LV:{--lvl}";
        onSkillLevelChanged.Invoke(1);
        SetSkillLockState();

        if (lvl <= 0 && equippedSlot != null)
        {
            equippedSlot.ClearSkill();
            equippedSlot = null;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (targetCanvas == null || !skillUnlocked || skill == null) return;
        
        // Create draggable icon instance
        draggableIcon = Instantiate(icon, targetCanvas.transform);
        draggableIcon.raycastTarget = false;
        draggableIcon.transform.position = icon.rectTransform.position;
        
        // Ensure dragged icon appears above other UI elements
        Canvas draggableCanvas = draggableIcon.gameObject.AddComponent<Canvas>();
        draggableCanvas.overrideSorting = true;
        draggableCanvas.sortingOrder = 10;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!skillUnlocked || skill == null) return;
        
        if (draggableIcon != null)
        {
            draggableIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!skillUnlocked || skill == null) return;
        
        if (draggableIcon != null)
        {
            // Destroy the draggable icon after the drag ends
            Destroy(draggableIcon.gameObject);
        }
    }

    public bool IsSkillUnlocked()
    {
        return skillUnlocked && skill != null;
    }

    public SkillSlot GetEquippedSlot()
    {
        return equippedSlot;
    }

    public void SetEquippedSlot(SkillSlot i)
    {
        equippedSlot = i;
    }

    public void SetEnabled(bool canUseSkillPoint)
    {
        UpdateButtonStates(canUseSkillPoint);
    }

    private void UpdateButtonStates(bool canUseSkillPoint)
    {
        bool hasSkill = skill != null;
        increment.interactable = canUseSkillPoint && hasSkill;
        decrement.interactable = lvl > 0 && hasSkill;
    }

    public void SetSkillLockState()
        {
        skillUnlocked = lvl > 0 && skill != null;
        blocker.SetActive(!skillUnlocked);
        UpdateButtonStates(increment.interactable);
    }
}
