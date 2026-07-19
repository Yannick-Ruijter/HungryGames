using UnityEngine;
using UnityEngine.InputSystem;

public class VegetableInteraction : MonoBehaviour
{
    private IInteractable _currentInteraction = null;
    private Collider _currentInteractionCollider = null;
    private PlayerInput _playerInput = null;
    private InputAction _interactInput = null;
    private PlayerController _playerController = null;
    private bool _isInteracting = false;
    private bool _interactionAssigned = false;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (!_interactionAssigned && _playerInput.actions != null)
        { 
            _interactInput = _playerInput.actions["Interact"];
            _interactInput.started += context => InteractionStarted();
            _interactInput.canceled += context => InteractionEnded();
            _interactionAssigned = true;
        }
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!_interactionAssigned && _playerInput.actions != null)
        {
            _interactInput = _playerInput.actions["Interact"];
            _interactInput.started += context => InteractionStarted();
            _interactInput.canceled += context => InteractionEnded();
            _interactionAssigned = true;
        }
    }
    void InteractionEnded()
    {
        if(_currentInteraction == null || !_isInteracting) return;
        StopInteracting();
    }
    void InteractionStarted()
    {
        if(_currentInteraction == null) return;
        _playerController.CanMove = false;
        _isInteracting = _currentInteraction.StartInteraction(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        IInteractable interaction;
        other.TryGetComponent<IInteractable>(out interaction);
        if (interaction == null) return;
        Debug.Log("Entered interactable area");
        _currentInteraction = interaction;
        _currentInteractionCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other != _currentInteractionCollider) return;
        _currentInteraction = null;
        _currentInteractionCollider = null;
    }

    public void StopInteracting()
    {
        _isInteracting = false;
        _playerController.CanMove = true;
        _currentInteraction?.StopInteraction(gameObject);
    }
}
