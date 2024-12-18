using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillBarSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private GameObject cooldownMask;
    private Image cooldownImage;
    private RectTransform cooldownImageRect;
    private TMP_Text cooldownTimeText;

    private Skill equippedSkill;

    private void Awake()
    {
        SetCooldownVisible(false);
    }

    private void Start()
    {
        cooldownImage = cooldownMask.GetComponentInChildren<Image>();
        cooldownImageRect = cooldownImage.GetComponent<RectTransform>();
        cooldownTimeText = cooldownMask.GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (equippedSkill != null)
        {
            if (!equippedSkill.CanCast())
            {
                UpdateCooldown();
            }
            else if (cooldownMask.activeSelf)
            {
                SetCooldownVisible(false);
            }
        }
    }

    public void UpdateSkill(Skill skill)
    {
        equippedSkill = skill;
        if (skill != null)
        {
            skillIcon.sprite = skill.icon;
            skillIcon.enabled = true;
            SetCooldownVisible(!skill.CanCast());
        }
        else
        {
            skillIcon.enabled = false;
            skillIcon.sprite = null;
            SetCooldownVisible(false);
        }
    }

    public void SetKeyText(string text)
    {
        if (keyText != null)
        {
            keyText.text = text;
        }
    }

    private void UpdateCooldown()
    {
        if (equippedSkill == null) return;

        float remainingTime = (equippedSkill.LastUsedTime + equippedSkill.cooldown) - Time.time;
        
        // remove cooldown mask
        if (remainingTime <= 0)
        {
            SetCooldownVisible(false);
            return;
        }

        // display cooldown mask
        if (!cooldownMask.activeSelf)
        {
            SetCooldownVisible(true);
        }

        // update cooldown mask height
        float cooldownRatio = remainingTime / equippedSkill.cooldown;
        float fullHeight = ((RectTransform)transform).rect.height;
        Vector2 sizeDelta = cooldownImageRect.sizeDelta;
        sizeDelta.y = fullHeight * cooldownRatio;
        cooldownImageRect.sizeDelta = sizeDelta;
        
        // update cooldown time text
        if (cooldownTimeText != null)
        {
            cooldownTimeText.text = remainingTime.ToString("F1");
        }
    }

    private void SetCooldownVisible(bool visible)
    {
        if (cooldownMask != null)
        {
            cooldownMask.SetActive(visible);
            if (visible)
            {
                // reset cooldown mask height
                float fullHeight = ((RectTransform)transform).rect.height;
                Vector2 sizeDelta = cooldownImageRect.sizeDelta;
                sizeDelta.y = fullHeight;
                cooldownImageRect.sizeDelta = sizeDelta;
            }
        }
    }
}
