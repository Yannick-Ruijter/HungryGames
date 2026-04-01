using UnityEngine;
using System.Collections.Generic;
public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private List<GameObject> _playerSlots = new();
    [SerializeField] private GameObject _selectionImage;
    [SerializeField] private float _selectedSizeMultiplier = 0.9f;
    private StartTextScript _startTextScript;
    private bool _selected = false;
    public bool HasBeenChosen { get { return _selected; } set { DoSelected(value);  _selected = value; } }

    private void Start()
    {
        _selectionImage.SetActive(false);
    }
    public Transform GetSlotTransform(int playerIndex)
    {
        return _playerSlots[playerIndex].transform;
    }

    void DoSelected(bool newValue)
    {
        if (newValue == _selected) return;
        _selected = newValue;
        if(_selected)
        {
            _selectionImage.SetActive(true);
            gameObject.transform.localScale *= _selectedSizeMultiplier;
        }
        else
        {
            _selectionImage.SetActive(false);
            gameObject.transform.localScale /= _selectedSizeMultiplier;
        }
    }
}
