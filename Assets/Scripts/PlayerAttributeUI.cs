using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerAttributeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text LabelText;
    [SerializeField] private TMP_Text ValueText;
    [SerializeField] private Button DecrementButton;
    [SerializeField] private Button IncrementButton;

    public string Title;

    public int Value;

    public UnityEvent<int> OnValueChanged;

    void Start() {
        IncrementButton.onClick.AddListener(() => {
            Value += 1;
            OnValueChanged.Invoke(Value);
        });

        DecrementButton.onClick.AddListener(() => {
            Value -= 1;
            OnValueChanged.Invoke(Value);
        });
    }

    // Update is called once per frame
    void Update()
    {
        LabelText.text = Title;
        ValueText.text = $"{Value}";
    }

    public void SetEnabled(bool val) {
        IncrementButton.interactable = val;
        DecrementButton.interactable = val;
    }
}
