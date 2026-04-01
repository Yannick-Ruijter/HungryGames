using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
public class PlayerManager : MonoBehaviour
{
    //static List<GameObject> players = new List<GameObject>();

    static Dictionary<PlayerInput, EntityMeshType> players = new Dictionary<PlayerInput, EntityMeshType>();
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        players.Add(playerInput, playerInput.gameObject.GetComponent<PlayerMainMenu>()._currentType);
        FindFirstObjectByType<ControllerDisplay>().AddPlayer(playerInput.gameObject);
    }

    public void SpawnPlayers()
    {
        foreach (var player in players)
        {
            Debug.Log(player);
        }

    }
}


