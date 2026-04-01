using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class ControllerDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<GameObject> _characterSlots = new();
    private List<CharacterSelection> _characterSelectionSlots = new();
    private List<PlayerMainMenu> _players = new();
    private StartTextScript _startText = null;
    private int _controllerCount = 0;
    private int _nrOfPlayersReady = 0;

    public bool CanGoRight(int charIndex)
    {
        for(int i = charIndex + 1; i < _characterSelectionSlots.Count; i++)
        {
            if (!_characterSelectionSlots[i].HasBeenChosen) return true;
        }
        return false;
    }

    public bool CanGoLeft(int charIndex)
    {
        for (int i = charIndex - 1; i >= 0; i--)
        {
            if (!_characterSelectionSlots[i].HasBeenChosen) return true;
        }
        return false;
    }

    public Transform GetNextTransform(ref int charIndex, int playerIndex)
    {
        if (charIndex == _characterSlots.Count - 1) return GetControllerPosition(charIndex, playerIndex);
        for (int i = charIndex + 1; i < _characterSlots.Count; ++i)
        {
            if (!_characterSelectionSlots[i].HasBeenChosen)
            {
                charIndex = i;
                break;
            }
        }
        return GetControllerPosition(charIndex, playerIndex);
    }

    public Transform GetPreviousTransform(ref int charIndex, int playerIndex)
    {
        if (charIndex == 0) return GetControllerPosition(charIndex, playerIndex);
        for (int i = charIndex - 1; i >= 0; i--)
        {
            if (!_characterSelectionSlots[i].HasBeenChosen)
            {
                charIndex = i;
                break;
            }
        }
        return GetControllerPosition(charIndex, playerIndex);
    }

    public Transform GetControllerPosition(int characterIndex, int playerIndex)
    {
        return _characterSelectionSlots[characterIndex].GetSlotTransform(playerIndex);
    }

    public EntityMeshType ToggleCharacterSelection(int characterIndex, GameObject player)
    {
        _characterSelectionSlots[characterIndex].HasBeenChosen = !_characterSelectionSlots[characterIndex].HasBeenChosen;
        PlayerMainMenu senderUiPlayer = player.GetComponent<PlayerMainMenu>();
        if (!_characterSelectionSlots[characterIndex].HasBeenChosen)
        {
            _nrOfPlayersReady--;
            _startText.PlayerInfoChanged(_nrOfPlayersReady, _players.Count);
            foreach (var p in _players) p.UpdateArrows();
            return EntityMeshType.None;
        }
        _nrOfPlayersReady++;
        _startText.PlayerInfoChanged(_nrOfPlayersReady, _players.Count);
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].UpdateArrows();
            if (player == _players[i].gameObject) continue;
            if (_players[i].SelectedCharIndex != senderUiPlayer.SelectedCharIndex) continue;
            Transform tempTransform = GetNextTransform(ref _players[i].SelectedCharIndex, i);
            if (player.transform.position == tempTransform.position)
            {
                tempTransform = GetPreviousTransform(ref _players[i].SelectedCharIndex, i);
            }
            _players[i].gameObject.transform.position = tempTransform.position;
        }
        return _characterSelectionSlots[characterIndex].CharType;
    }

    public void AddPlayer(GameObject player)
    {
        _players.Add(player.GetComponent<PlayerMainMenu>());
        _startText.PlayerInfoChanged(_nrOfPlayersReady, _players.Count);
    }
    void Start()
    {
        _characterSlots = _characterSlots.OrderBy(x => x.transform.position.x).ToList();
        foreach (var slot in _characterSlots)
        {
            _characterSelectionSlots.Add(slot.GetComponent<CharacterSelection>());
        }
        _startText = FindFirstObjectByType<StartTextScript>();
    }
}
