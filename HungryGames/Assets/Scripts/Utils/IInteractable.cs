using UnityEngine;

public interface IInteractable
{
    bool StartInteraction(GameObject player);
    void StopInteraction(GameObject player);
}
