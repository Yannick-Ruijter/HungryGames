using UnityEngine;
using System.Collections.Generic;
public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private List<GameObject> _playerSlots = new();
    public bool HasBeenChosen = false;
    public Transform GetSlotTransform(int playerIndex)
    {
        return _playerSlots[playerIndex].transform;
    }
}
