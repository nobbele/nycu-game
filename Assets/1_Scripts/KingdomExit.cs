using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KingdomExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player)) return;
        
        if (SaveSystem.ClearedAllBosses()) GoToCredits();
        else GoToHub();
    }

    private void GoToHub()
    {
        SceneManager.LoadScene("MainHubScene");
    }

    private void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
