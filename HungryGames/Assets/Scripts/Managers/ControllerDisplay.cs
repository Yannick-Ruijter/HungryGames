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
    private List<GameObject> _players = new();
    private int _controllerCount = 0;   

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

    public void ToggleCharacterSelection(int characterIndex, GameObject player)
    {
        _characterSelectionSlots[characterIndex].HasBeenChosen = !_characterSelectionSlots[characterIndex].HasBeenChosen;
        PlayerMainMenu senderUiPlayer = player.GetComponent<PlayerMainMenu>();
        if (!_characterSelectionSlots[characterIndex].HasBeenChosen) return;
        for(int i = 0; i < _players.Count; i++)
        {
            if (player == _players[i]) continue;
            PlayerMainMenu currentUiPlayer = _players[i].GetComponent<PlayerMainMenu>();
            if (currentUiPlayer.SelectedCharIndex != senderUiPlayer.SelectedCharIndex) return;
            Transform tempTransform = GetNextTransform(ref currentUiPlayer.SelectedCharIndex, i);
            if (player.transform.position == tempTransform.position)
            {
                tempTransform = GetPreviousTransform(ref currentUiPlayer.SelectedCharIndex, i);
            }
            _players[i].transform.position = tempTransform.position;
        }
    }

    public void AddPlayer(GameObject player)
    {
        _players.Add(player);
    }
    void Start()
    {
        _characterSlots = _characterSlots.OrderBy(x => x.transform.position.x).ToList();
        foreach (var slot in _characterSlots)
        {
            _characterSelectionSlots.Add(slot.GetComponent<CharacterSelection>());
        }
    }
}
