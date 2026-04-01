using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class PlayerMainMenu : MonoBehaviour
{
    static int PlayerCount = 0;
    [SerializeField] private int _nrOfChars = 5;
    [SerializeField] private List<Color> _selectionColors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow, Color.azure };
    private int _playerNr;
    private int _selectedCharIndex = 0;
    private ControllerDisplay _controllerDisplay = null;
    public int PlayerNr { get { return _playerNr; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerNr = PlayerCount;
        _selectedCharIndex = _playerNr;
        _controllerDisplay = FindFirstObjectByType<ControllerDisplay>();
        gameObject.transform.position = _controllerDisplay.GetControllerPosition(_selectedCharIndex, _playerNr).position;
        gameObject.transform.SetParent(_controllerDisplay.gameObject.transform, true);
        gameObject.transform.localScale = Vector3.one;
        PlayerCount++;
    }

    public void NavigateMenu(InputAction.CallbackContext context )
    {
        if(context.started)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input.x > 0)
            {
                gameObject.transform.position = _controllerDisplay.GetNextTransform(ref _selectedCharIndex, _playerNr).position;
            }
            else if (input.x < 0)
            {
                gameObject.transform.position = _controllerDisplay.GetPreviousTransform(ref _selectedCharIndex, _playerNr).position;
            }
        }
    }
}
