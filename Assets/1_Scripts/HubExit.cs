using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubExit : MonoBehaviour
{
    public string TargetSceneName;
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player)) return;
        
        // TODO: Transfer player stats
        SceneManager.LoadScene(TargetSceneName);
    }
}
