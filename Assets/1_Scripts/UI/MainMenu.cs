using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void PlayGame() {
        // SceneManager.LoadScene("MainScene");
        SceneManager.LoadScene("MainHubScene");
        Debug.Log("Play Game");
    }

    public void OpenSettings() {
        SceneManager.LoadScene("SettingsMenuScene");
    }

    public void QuitGame() {
        // TODO: Implement "quit game"
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
