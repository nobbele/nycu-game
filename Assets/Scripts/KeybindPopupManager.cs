using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindPopupManager : MonoBehaviour
{
    public GameObject popupPanel;

    public KeyBinding keyBinding;

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        keyBinding.Load();
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
