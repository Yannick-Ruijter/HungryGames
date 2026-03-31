using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeviceVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject m_ControllerVisualPrefab;
    
    private Dictionary<InputDevice, DeviceUIData> devices = new Dictionary<InputDevice, DeviceUIData>();

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        InputSystem.actions["StartGame"].performed += StartGame;
        
        foreach (InputDevice device in InputSystem.devices) 
            OnDeviceChange(device, InputDeviceChange.Added);
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        InputSystem.actions["StartGame"].performed -= StartGame;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (!(device is Gamepad))
            return;
        if (change == InputDeviceChange.Added)
        {
            GameObject instance = Instantiate(m_ControllerVisualPrefab, transform);
            
            TMPro.TMP_Text text = instance.GetComponentInChildren<TMPro.TMP_Text>();

            if (text)
                text.text = device.name;

            DeviceUIData data = new DeviceUIData()
            {
                isMaster = devices.Count == 0,
                device = device,
                instance = instance
            };
            
            devices.Add(device, data);
        }
        else if (change == InputDeviceChange.Removed)
        {
            DeviceUIData data = devices[device];

            if (data.isMaster)
            {
                foreach (var element in devices.Values)
                {
                    element.isMaster = true;
                    break;
                }
            }
            
            Destroy(data.instance);
        }
    }

    private void StartGame(InputAction.CallbackContext ctx)
    {
        InputDevice device = ctx.control.device;
        
        DeviceUIData data = devices[device];

        if (data != null && data.isMaster)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    public class DeviceUIData
    {
        public bool isMaster;
        public InputDevice device;
        public GameObject instance;
    }
}
