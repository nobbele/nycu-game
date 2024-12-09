using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthDisplay : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text healthPercent;

    public float HealthPercent {
        get => healthBarSlider.value;
        set {
            healthBarSlider.value = value;
            healthPercent.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }
}