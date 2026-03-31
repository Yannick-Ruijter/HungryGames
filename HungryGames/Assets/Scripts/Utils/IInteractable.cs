using UnityEngine;

public interface IInteractable
{
    void StartInteraction(GameObject player);
    void StopInteraction(GameObject player);
}
