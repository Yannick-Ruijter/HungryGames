using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterState
{
    public InputDevice device;
    public PlayerInput input;
    public bool isFarmer;
}

public class PlayerJoinManager : MonoBehaviour
{
    private List<PlayerCharacterState> m_PlayerCharacterStates = new List<PlayerCharacterState>();
    [SerializeField] private PlayerInputManager m_PlayerInputManager;

    [SerializeField] private PlayerInput m_PlayerControllerPrefab;
    [SerializeField] private PlayerInput m_FarmerControllerPrefab;

    private void Start()
    {
        // InputSystem.onDeviceChange += OnDeviceChange;

        foreach (InputDevice device in InputSystem.devices)
        {
            OnDeviceChange(device, InputDeviceChange.Added);
        }
    }

    private void OnGUI()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            GUILayout.Label(device.name);
        }
    }

    private void OnDestroy()
    {
        // InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad))
            return;
        if (change == InputDeviceChange.Added)
        {
            bool willBeFarmer = m_PlayerCharacterStates.Count == 0;
            bool farmerExists = false;
            foreach (PlayerCharacterState playerCharacterState in m_PlayerCharacterStates)
                if (playerCharacterState.isFarmer)
                {
                    farmerExists = true;
                    break;
                }

            if (!farmerExists)
                willBeFarmer = true;

            m_PlayerInputManager.playerPrefab =
                willBeFarmer ? m_FarmerControllerPrefab.gameObject : m_PlayerControllerPrefab.gameObject;
            
            m_PlayerCharacterStates.Add(new PlayerCharacterState()
            {
                device = device,
                input = m_PlayerInputManager.JoinPlayer(-1, -1, null, device),
                isFarmer = willBeFarmer
            });
        }
        else if (change == InputDeviceChange.Removed)
        {
            foreach (PlayerCharacterState playerCharacterState in m_PlayerCharacterStates)
            {
                if (playerCharacterState.device == device)
                {
                    m_PlayerCharacterStates.Remove(playerCharacterState);
                    Destroy(playerCharacterState.input.gameObject);
                    return;
                }
            }
        }
    }
}
