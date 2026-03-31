using UnityEngine;

public class MineController : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionTimeNeeded = 10f;
    private float _currentInteractionTime = 0f;
    private float _resetRate = 0f;
    private int _nrOfPlayersInteracting = 0;
    public void StartInteraction(GameObject player)
    {
        _nrOfPlayersInteracting++;
    }

    public void StopInteraction(GameObject player)
    {
        _nrOfPlayersInteracting--;
    }
    // Update is called once per frame
    void Update()
    {
        if(_nrOfPlayersInteracting > 0)
        {
            _currentInteractionTime += Time.deltaTime * _nrOfPlayersInteracting;
        }
        else if(_currentInteractionTime > 0f)
        {
            _currentInteractionTime -= Time.deltaTime * _resetRate;
            if (_currentInteractionTime < 0f) _currentInteractionTime = 0f;
        }
    }
}
