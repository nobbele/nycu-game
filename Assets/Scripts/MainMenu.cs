using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void PlayGame() {
        // TODO: Implement "play game"
        // SceneManager.LoadScene("MainScene");
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
