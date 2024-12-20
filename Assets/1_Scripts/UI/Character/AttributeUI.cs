using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttributeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private Button DecrementButton;
    [SerializeField] private Button IncrementButton;

    public int Value { get; private set; }
    
    // Add reference to PlayerStats
    private PlayerStats playerStats;
    
    public UnityEvent<int> OnValueChanged;

    void Start() {
        IncrementButton.onClick.AddListener(() => {
            if (playerStats.AttributePoints > 0) {
                Value += 1;
                playerStats.AttributePoints -= 1;
                OnValueChanged.Invoke(Value);
                UpdateButtonStates();
            }
        });

        DecrementButton.onClick.AddListener(() => {
            if (Value > 0) {
                Value -= 1;
                playerStats.AttributePoints += 1;
                OnValueChanged.Invoke(Value);
                UpdateButtonStates();
            }
        });
        
        UpdateButtonStates();
    }

    void Update()
    {
        ValueText.text = $"{Value}";
        UpdateButtonStates();
    }
    
    private void UpdateButtonStates() {
        if (playerStats != null) {
            IncrementButton.interactable = playerStats.AttributePoints > 0;
            DecrementButton.interactable = Value > 0;
        }
    }

    public void SetValue(int newValue) {
        Value = newValue;
        UpdateButtonStates();
    }
    
    public void Initialize(PlayerStats stats) {
        playerStats = stats;
        UpdateButtonStates();
    }
}
