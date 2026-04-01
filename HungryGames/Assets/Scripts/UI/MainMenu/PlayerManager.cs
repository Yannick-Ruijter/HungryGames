using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        FindFirstObjectByType<ControllerDisplay>().AddPlayer(playerInput.gameObject);
    }
}
