using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;
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
        string selectedLanguage = languageDropdown.options[languageDropdown.value].text;
        PlayerPrefs.SetString("Language", selectedLanguage);
        PlayerPrefs.Save();

        ChangeLanguage(selectedLanguage);
    }

    private void ChangeLanguage(string lang)
    {
        string localeCode = lang switch
        {
            "English" => "en",
            "中文" => "zh-TW",
            "Svenska" => "sv",
            _ => "en"
        };
        StartCoroutine(SetLocale(localeCode));
    }

    private IEnumerator SetLocale(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;
        var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        LocalizationSettings.SelectedLocale = selectedLocale;
    }

    public void GoBackMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
