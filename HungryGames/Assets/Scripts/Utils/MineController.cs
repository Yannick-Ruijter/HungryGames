using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class MineController : MonoBehaviour, IInteractable
{
    public static List<MineController> Mines { get; private set; } = new List<MineController>();
    [SerializeField] private float _interactionTimeNeeded = 10f;
    private float _currentInteractionTime = 0f;
    [SerializeField] private float _resetRate = 1f;
    private int _nrOfPlayersInteracting = 0;
    private bool _isDefused = false;
    List<GameObject> _interactingPlayers = new();
    List<GameObject> _toBeRemovedPlayers = new();
    [SerializeField] public Renderer _renderer;
    List<Material> _materials = new List<Material>();

    private void Start()
    {
        Mines.Add(this);
        _renderer.GetMaterials(_materials);
        Debug.Log(_materials);
    }

    private void OnDestroy()
    {
        Mines.Remove(this);
    }

    public bool StartInteraction(GameObject player)
    {
        Debug.Log("Started interacting");
        if (_isDefused) return false;
        _nrOfPlayersInteracting++;
        _interactingPlayers.Add(player);
        return true;
    }

    public void StopInteraction(GameObject player)
    {
        _nrOfPlayersInteracting--;
        _toBeRemovedPlayers.Add(player);
    }
    // Update is called once per frame
    void Update()
    {
        bool changeMat = false;
        if (_nrOfPlayersInteracting > 0)
        {
            changeMat = true;
            _currentInteractionTime += Time.deltaTime * _nrOfPlayersInteracting;
            if (_currentInteractionTime >= _interactionTimeNeeded && !_isDefused)
            {
                _isDefused = true;
                foreach (var player in _interactingPlayers)
                {
                    player.GetComponent<VegetableInteraction>().StopInteracting();
                }
                Debug.Log("done interacting!");
                GameState.Instance.OnMineDefuse(this);
            }
        }
        else if (_currentInteractionTime > 0f)
        {
            changeMat = true;
            _currentInteractionTime -= Time.deltaTime * _resetRate;
            if (_currentInteractionTime < 0f) _currentInteractionTime = 0f;
        }

        // if he's not diffused and the value changed, update material
        if (changeMat && !_isDefused)
        {
            Debug.Log("changing");
            Debug.Log(_currentInteractionTime);
            _materials[2].SetFloat("_time", _currentInteractionTime/ _interactionTimeNeeded);
        }

        if (_toBeRemovedPlayers.Count > 0)
        {
            foreach (var player in _toBeRemovedPlayers)
            {
                _interactingPlayers.Remove(player);
            }
            _toBeRemovedPlayers.Clear();
        }
    }
}
