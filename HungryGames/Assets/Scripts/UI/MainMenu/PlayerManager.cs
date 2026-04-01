using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;
public class PlayerManager : MonoBehaviour
{
    //static List<GameObject> players = new List<GameObject>();

    class PlayerData
    {
        public InputUser user;
        public InputDevice device;
        public EntityMeshType type;
        public int playerIndex;
    }

    static List<PlayerData> players = new List<PlayerData>();

    [SerializeField] private PlayerInputManager m_PlayerInputManager;
    [SerializeField] private GameObject _vegetable;
    [SerializeField] private GameObject _farmer;



    //List<PlayerInput> playerInputs = new List<PlayerInput>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //SceneManager.activeSceneChanged += OnSceneSwitch;
    }
    private void Start()
    {
        if(m_PlayerInputManager != null)
        m_PlayerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        var data = new PlayerData
        {
            device = playerInput.devices[0],
            type = playerInput.GetComponent<PlayerMainMenu>()._currentType,
            playerIndex = playerInput.playerIndex
        };

        players.Add(data);

        FindFirstObjectByType<ControllerDisplay>().AddPlayer(playerInput.gameObject);
    }

    //private void OnSceneSwitch(Scene arg0, Scene arg1)
    //{
    //    Debug.Log("hi");
    //    //m_PlayerInputManager\
    //    //p.pl
    //    Debug.Log(players);
    //    foreach (var player in players)
    //    {
    //        Debug.Log(player);
    //    }
    //    FindAnyObjectByType<GameState>()._playerManager = this;

    //}

    public void SpawnPlayers()
    {
        foreach (var player in players)
        {
            GameObject prefab = player.type == EntityMeshType.Farmer
                ? _farmer
                : _vegetable;

            PlayerInput.Instantiate(
                prefab,
                controlScheme: "Gamepad",
                pairWithDevice: player.device
            );
        }
    }

    public void OnCharacterSelected()
    {
        // do sounds things or whatever here
        Debug.Log("Character has been selected");
    }

    public void OnPlayerJoined()
    {
        //do sounds here
        Debug.Log("Player joined");
    }

    public void OnPlayerLeft()
    {
        Debug.Log("Player left");
    }

    public void OnCharacterDeselected()
    {
        Debug.Log("Character has been DEselected");
    }

    public void OnCharacterChanged()
    {
        Debug.Log("player switched to another character");
    }

}


