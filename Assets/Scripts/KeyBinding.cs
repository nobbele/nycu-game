using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyBinding : MonoBehaviour
{
    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>
        {
            { "Forward", KeyCode.W },
            { "Left", KeyCode.A },
            { "Backward", KeyCode.S },
            { "Right", KeyCode.D }
        };
    private Dictionary<string, TMP_InputField> keybindInputs = new Dictionary<string, TMP_InputField>();

    public void Load()
    {
        LoadKeyBindings();
        InitializeKeybindInputs();
    }

    public void LoadKeyBindings()
    {
        var keys = new List<string>(keybinds.Keys);
        foreach (var key in keys)
        {
            string savedKey = PlayerPrefs.GetString($"Keybind_{key}", keybinds[key].ToString());
            if (System.Enum.TryParse(savedKey, out KeyCode keyCode))
            {
                keybinds[key] = keyCode;
            }
        }
    }

    private void SaveKeyBindings()
    {
        foreach (var keybind in keybinds)
        {
            PlayerPrefs.SetString($"Keybind_{keybind.Key}", keybind.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    private void InitializeKeybindInputs()
    {
        keybindInputs = new Dictionary<string, TMP_InputField>();
        foreach (var key in keybinds.Keys)
        {
            TMP_InputField inputField = CreateOrGetInputField(key);
            if (inputField != null)
            {
                inputField.text = keybinds[key].ToString();
                inputField.onValueChanged.AddListener(newValue => OnKeyInputChanged(key, newValue));
                keybindInputs[key] = inputField;
            }
        }
    }

    private TMP_InputField CreateOrGetInputField(string action)
    {
        GameObject inputObject = GameObject.Find($"KeyInput_{action}");
        if (inputObject != null)
        {
            return inputObject.GetComponent<TMP_InputField>();
        }
        return null;
    }

    private void OnKeyInputChanged(string action, string newKey)
    {
        if (string.IsNullOrEmpty(newKey)) return;

        newKey = newKey[newKey.Length - 1].ToString().ToUpper();
        if (System.Enum.TryParse(newKey, out KeyCode keyCode))
        {
            keybinds[action] = keyCode;
            keybindInputs[action].text = newKey;
            SaveKeyBindings();
        }
    }

    void OnDestroy()
    {
        foreach (var input in keybindInputs.Values)
        {
            input.onValueChanged.RemoveAllListeners();
        }
    }
}
