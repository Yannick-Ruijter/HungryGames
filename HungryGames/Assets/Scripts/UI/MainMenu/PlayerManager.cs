using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    //static List<GameObject> players = new List<GameObject>();

    class PlayerData
    {
        public InputUser user;
        public InputDevice device;
        public EntityMeshType type;
        public int playerIndex;
        public GameObject gameObject;
    }

    static List<PlayerData> players = new List<PlayerData>();

    [SerializeField] private PlayerInputManager m_PlayerInputManager;
    [SerializeField] private GameObject _vegetable;
    [SerializeField] private GameObject _farmer;



    //List<PlayerInput> playerInputs = new List<PlayerInput>();

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        if(m_PlayerInputManager != null)
        SceneManager.activeSceneChanged += OnSceneSwitch;
        m_PlayerInputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        var data = new PlayerData
        {
            user = playerInput.user,
            device = playerInput.devices[0],
            type = playerInput.GetComponent<PlayerMainMenu>().CurrentType,
            playerIndex = playerInput.playerIndex,
            gameObject = playerInput.gameObject
        };

        players.Add(data);

        FindFirstObjectByType<ControllerDisplay>().AddPlayer(playerInput.gameObject);
    }

    private void OnSceneSwitch(Scene arg0, Scene arg1)
    {
        Debug.Log("hi");

        Debug.Log(players);
        foreach (var player in players)
        {
            //player.type = player.gameObject.GetComponent<PlayerMainMenu>().CurrentType;
            Debug.Log(player);
        }
        FindAnyObjectByType<GameState>()._playerManager = this;

    }

    public void SpawnPlayers()
    {
        var vegetableSpawnPoints = FindObjectsByType<VegetableSpawnPoint>(FindObjectsSortMode.None);
        var farmerSpawnPoint = FindFirstObjectByType<FarmerSpawnPoint>();
        int spawnpointIndex = 0;
        foreach (var player in players)
        {
            Debug.Log(player);
            GameObject prefab = player.type == EntityMeshType.Farmer
                ? _farmer
                : _vegetable;

            var newPlayer = PlayerInput.Instantiate(
                prefab,
                controlScheme: "Gamepad",
                pairWithDevice: player.device
            );
            if(player.type == EntityMeshType.Farmer)
            {
                newPlayer.gameObject.transform.position = farmerSpawnPoint.transform.position;
            }
            else
            {
                newPlayer.gameObject.transform.position = vegetableSpawnPoints[spawnpointIndex].transform.position;
                spawnpointIndex++;
            }
            newPlayer.gameObject.tag = "Vegetable";

            newPlayer.gameObject.GetComponent<Entity>().AssignType(player.type);
        }
    }

    public void OnGameStart()
    {
        foreach (var player in players)
        {
            player.type = player.gameObject.GetComponent<PlayerMainMenu>().CurrentType;
            Debug.Log(player);
        }
    }
    public void OnCharacterSelected()
    {
        PlayerAudioManager.PlaySoundNonAlloc("SFX_UI_CharSelected");
        Debug.Log("Character has been selected");
    }

    public void OnPlayerJoined()
    {
        PlayerAudioManager.PlaySoundNonAlloc("SFX_UI_PlayerJoined");
        Debug.Log("Player joined");
    }

    public void OnPlayerLeft()
    {
        PlayerAudioManager.PlaySoundNonAlloc("SFX_UI_PlayerLeft");
        Debug.Log("Player left");
    }

    public void OnCharacterDeselected()
    {
        PlayerAudioManager.PlaySoundNonAlloc("SFX_UI_CharDeselected");
        Debug.Log("Character has been DEselected");
    }

    public void OnCharacterChanged()
    {
        PlayerAudioManager.PlaySoundNonAlloc("SFX_UI_CharChanged");
        Debug.Log("player switched to another character");
    }

}


