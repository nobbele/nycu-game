using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Button loadButton;
    
    void Start()
    {
        loadButton.interactable = SaveSystem.HasSave && !SaveSystem.ClearedAllBosses();
    }
    
    public void PlayGame() {
        Debug.Log("New Game");
        SaveSystem.ResetSaveData();
        SceneManager.LoadScene("MainHubScene");
    }
    
    public void LoadGame() {
        Debug.Log("Load Game");
        SceneManager.LoadScene("MainHubScene");
    }

    public void OpenSettings() {
        SceneManager.LoadScene("SettingsMenuScene");
    }

    public void QuitGame() {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
