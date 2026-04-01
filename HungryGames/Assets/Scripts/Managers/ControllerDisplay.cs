using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class ControllerDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private List<GameObject> _characterSlots = new();
    private List<CharacterSelection> _characterSelectionSlots = new();
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

    void Start()
    {
        _characterSlots = _characterSlots.OrderBy(x => x.transform.position.x).ToList();
        foreach (var slot in _characterSlots)
        {
            _characterSelectionSlots.Add(slot.GetComponent<CharacterSelection>());
        }
    }
}
