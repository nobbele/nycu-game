using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;
    
    private PlayerUIController playerUIController;

    private void Awake()
    {
        gameObject.SetActive(false);
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideUI);
        }
    }

    public void Initialize(PlayerUIController controller)
    {
        playerUIController = controller;
    }

    private void HideUI()
    {
        if (playerUIController != null)
        {
            playerUIController.HideUI(gameObject);
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideUI);
        }
    }
}
