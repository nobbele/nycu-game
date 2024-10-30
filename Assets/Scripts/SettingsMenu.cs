using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Dropdown languageDropdown;

    void Start()
    {
        LoadSettings();
    }

    public void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);

        string savedLanguage = PlayerPrefs.GetString("Language", "English");
        int index = languageDropdown.options.FindIndex(option => option.text == savedLanguage);
        languageDropdown.value = index >= 0 ? index : 0;
    }

    public void OnVolumeChanged()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.Save();
    }

    public void OnLanguageChanged()
    {
        PlayerPrefs.SetString("Language", languageDropdown.options[languageDropdown.value].text);
        PlayerPrefs.Save();
    }

    public void GoBackMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
