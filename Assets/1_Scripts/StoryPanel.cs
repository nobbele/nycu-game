using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryPanel : MonoBehaviour
{
    public void GoPortalScene()
    {
        SceneManager.LoadScene("PortalHubScene");
    }
}
