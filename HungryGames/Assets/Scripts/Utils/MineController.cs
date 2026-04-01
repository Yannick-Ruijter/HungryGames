using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class MineController : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionTimeNeeded = 10f;
    private float _currentInteractionTime = 0f;
    private float _resetRate = 0f;
    private int _nrOfPlayersInteracting = 0;
    private bool _isDefused = false;
    List<GameObject> _interactingPlayers = new();
    List<GameObject> _toBeRemovedPlayers = new();

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
        if(_nrOfPlayersInteracting > 0)
        {
            _currentInteractionTime += Time.deltaTime * _nrOfPlayersInteracting;
            if (_currentInteractionTime >= _interactionTimeNeeded && !_isDefused)
            {
                _isDefused = true;
                foreach (var player in _interactingPlayers)
                {
                    player.GetComponent<VegetableInteraction>().StopInteracting();
                }
                Debug.Log("done interacting!");
            }
        }
        else if(_currentInteractionTime > 0f)
        {
            _currentInteractionTime -= Time.deltaTime * _resetRate;
            if (_currentInteractionTime < 0f) _currentInteractionTime = 0f;
        }
        if(_toBeRemovedPlayers.Count > 0)
        {
            foreach (var player in _toBeRemovedPlayers)
            {
                _interactingPlayers.Remove(player);
            }
            _toBeRemovedPlayers.Clear();
        }
    }
}
