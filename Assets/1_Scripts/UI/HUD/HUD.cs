using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private BarScaler healthBarScaler;
    [SerializeField] private BarScaler experienceBarScaler;
    [SerializeField] private TMP_Text LevelLabel;

    private void Start()
    {
        if (healthBarScaler == null)
        {
            Debug.LogWarning("Health Bar Scaler not assigned in HUD!");
        }
        if (experienceBarScaler == null)
        {
            Debug.LogWarning("Experience Bar Scaler not assigned in HUD!");
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarScaler != null)
        {
            healthBarScaler.SetMaxValue(maxHealth);
            healthBarScaler.SetValue(currentHealth);
        }
    }

    public void UpdateExperience(float currentExp, float requiredExp, int level)
    {
        if (experienceBarScaler != null)
        {
            experienceBarScaler.SetMaxValue(requiredExp);
            experienceBarScaler.SetValue(currentExp);
        }
        LevelLabel.text = $"Level: {level + 1}";
    }
}
