using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthPercent;
    [SerializeField] private TMP_Text bossName;

    public string BossName {
        get => bossName.text;
        set => bossName.text = value;
    }

    public float HealthPercent {
        get => healthBarSlider.value;
        set {
            healthBarSlider.value = value;
            healthPercent.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }
}
