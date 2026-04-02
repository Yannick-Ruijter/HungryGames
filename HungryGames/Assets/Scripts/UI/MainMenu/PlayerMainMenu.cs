
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TMPro;
public class PlayerMainMenu : MonoBehaviour
{
    static int PlayerCount = 0;
    [SerializeField] private List<Color> _selectionColors = new List<Color> { Color.red, Color.green, Color.blue, Color.yellow, Color.azure };
    [SerializeField] private TextMeshProUGUI _text = null;
    [SerializeField] private GameObject _rightArrow = null;
    [SerializeField] private GameObject _leftArrow = null;
    public EntityMeshType CurrentType = EntityMeshType.None;
    private bool _characterSelected = false;
    public bool CharacterSelected { get { return _characterSelected; } }
    private PlayerManager _playerManager;
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
        _rightArrow.SetActive(_controllerDisplay.CanGoRight(SelectedCharIndex));
        _leftArrow.SetActive(_controllerDisplay.CanGoLeft(SelectedCharIndex));
        _playerManager = FindFirstObjectByType<PlayerManager>();
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
        _rightArrow.SetActive(_controllerDisplay.CanGoRight(SelectedCharIndex));
        _leftArrow.SetActive(_controllerDisplay.CanGoLeft(SelectedCharIndex));
    }

    public void SelectCharacter(InputAction.CallbackContext context)
    {
        if (_controllerDisplay == null) return;
        if (context.started)
        {
            _characterSelected = !_characterSelected;
            _controllerDisplay.ToggleCharacterSelection(SelectedCharIndex, this);
        }
    }

    public void UpdateArrows()
    {
        if(_characterSelected)
        {
            _rightArrow.SetActive(false);
            _leftArrow.SetActive(false);
        }
        else
        {
            _rightArrow.SetActive(_controllerDisplay.CanGoRight(SelectedCharIndex));
            _leftArrow.SetActive(_controllerDisplay.CanGoLeft(SelectedCharIndex));
        }
    }

    public void StartGame(InputAction.CallbackContext context)
    {
        if (context.started) _controllerDisplay.StartGame();
    }
}
