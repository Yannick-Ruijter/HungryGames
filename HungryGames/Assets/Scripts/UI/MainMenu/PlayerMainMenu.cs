using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TMPro;
public class PlayerMainMenu : MonoBehaviour
{
    static int PlayerCount = 0;
    [SerializeField] private int _nrOfChars = 5;
    [SerializeField] private List<Color> _selectionColors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow, Color.azure };
    [SerializeField] private TextMeshProUGUI _text = null;
    private bool _characterSelected = false;
    public bool CharacterSelected { get { return _characterSelected; } }
    private int _playerNr;
    public int SelectedCharIndex = 0;
    private ControllerDisplay _controllerDisplay = null;
    public int PlayerNr { get { return _playerNr; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerNr = PlayerCount;
        SelectedCharIndex = -1;
        _text.text = "P" + (_playerNr + 1);
        _text.color = _selectionColors[_playerNr];
        _controllerDisplay = FindFirstObjectByType<ControllerDisplay>();
        gameObject.transform.position = _controllerDisplay.GetNextTransform(ref SelectedCharIndex, _playerNr).position;
        gameObject.transform.SetParent(_controllerDisplay.gameObject.transform, true);
        gameObject.transform.localScale = Vector3.one;
        PlayerCount++;
    }

    public void NavigateMenu(InputAction.CallbackContext context )
    {
        if (_characterSelected) return;
        if (_controllerDisplay == null) return;
        if(context.started)
        {
            Vector2 input = context.ReadValue<Vector2>();
            if (input.x > 0)
            {
                gameObject.transform.position = _controllerDisplay.GetNextTransform(ref SelectedCharIndex, _playerNr).position;
            }
            else if (input.x < 0)
            {
                gameObject.transform.position = _controllerDisplay.GetPreviousTransform(ref SelectedCharIndex, _playerNr).position;
            }
        }
    }

    public void SelectCharacter(InputAction.CallbackContext context)
    {
        if (_controllerDisplay == null) return;
        if (context.started)
        {
            _characterSelected = !_characterSelected;
            _controllerDisplay.ToggleCharacterSelection(SelectedCharIndex, gameObject);
        }
    }
}
