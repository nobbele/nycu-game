using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private Image icon;
    [SerializeField] private Button increment;
    [SerializeField] private Button decrement;
    [SerializeField] private GameObject blocker;

    public UnityEvent<int> onSkillLevelChanged = new();
    
    public Skill skill;
    private SkillSlot equippedSlot;
    private Image draggableIcon;
    private Transform originalParent;
    private int lvl = 0;
    private bool skillUnlocked = false;
    
    private void Awake()
    {
        SetSkillLockState();
        increment.onClick.AddListener(AddLvl);
        decrement.onClick.AddListener(ReduceLvl);
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        nameText.text = skill.skillName;
        icon.sprite = skill.icon;
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
        originalParent = transform.parent;
        if (!skillUnlocked) return;
        
        draggableIcon = Instantiate(icon, transform.root);
        draggableIcon.raycastTarget = false;
        draggableIcon.transform.position = icon.rectTransform.position;
        draggableIcon.canvas.sortingOrder = 10;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!skillUnlocked) return;
        
        if (draggableIcon != null)
        {
            draggableIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!skillUnlocked)
        {
            transform.SetParent(originalParent);
            return;
        }
        
        if (draggableIcon != null)
        {
            // Destroy the draggable icon after the drag ends
            Destroy(draggableIcon.gameObject);
        }
        
        transform.SetParent(originalParent); // Return to original parent if no valid drop target
    }

    public bool IsSkillUnlocked()
    {
        return skillUnlocked;
    }

    public SkillSlot GetEquippedSlot()
    {
        return equippedSlot;
    }

    public void SetEquippedSlot(SkillSlot i)
    {
        equippedSlot = i;
    }

    public void SetEnabled(bool b)
    {
        increment.interactable = b;
        decrement.interactable = lvl > 0;
    }

    public void SetSkillLockState()
    {
        if (lvl < 1) {
            blocker.SetActive(true);
            skillUnlocked = false;
        }
        else
        {
            blocker.SetActive(false);
            skillUnlocked = true;
        }
    }
}
