using UnityEngine;

public interface IInteractable
{
    // Check if player can interact with this object
    bool CanInteract(GameObject player);
    
    // Called when player interacts with this object
    void OnInteract(GameObject player);
    
    // Get interact prompt text
    string GetInteractPrompt();
}
