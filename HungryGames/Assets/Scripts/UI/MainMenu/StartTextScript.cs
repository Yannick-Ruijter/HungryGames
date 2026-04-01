using TMPro;
using UnityEngine;

public class StartTextScript : MonoBehaviour
{
    [SerializeField] private float _flickerFrequency = 2f;
    [SerializeField] private TextMeshProUGUI _text;
    private ControllerDisplay _display;
    private int _nrOfPlayers = 0;
    private int _nrPlayersReady = 0;
    private bool _playersReady;
    bool _textVisible = true;

    private void Start()
    {
        _text.text = "Press anything to join";
        _display = FindFirstObjectByType<ControllerDisplay>();
    }

    public void PlayerInfoChanged(int playersSelected, int nrOfPlayers)
    {
        _nrOfPlayers = nrOfPlayers;
        _nrPlayersReady = playersSelected;
        _playersReady = (_nrPlayersReady == _nrOfPlayers);
        if (!_playersReady) ShowNotReady();
        else ShowReady();
    }
     
    private void ShowNotReady()
    {
        CancelInvoke();
        _textVisible = true;
        _text.gameObject.SetActive(true);
        _text.text = "Players ready: " + _nrPlayersReady + "/" + _nrOfPlayers;
    }

    private void ShowReady()
    {
        _textVisible = true;
        _text.gameObject.SetActive(_textVisible);
        if (_nrOfPlayers < 2)
        {
            _text.text = "You need at least 2 players";
        }
        else if(!_display.IsFarmerPresent())
        {
            _text.text = "You need at least 1 farmer";
        }
        else
        {
            _text.text = "Press North Button to start!";
            InvokeRepeating("Flicker", _flickerFrequency, _flickerFrequency);
        }

    }

    private void Flicker()
    {
        _textVisible = !_textVisible;
        _text.gameObject.SetActive(_textVisible);
    }
}
