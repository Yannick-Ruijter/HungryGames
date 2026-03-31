using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Diagnostics;
public class FarmerController : PlayerController
{
    [SerializeField] GameObject _hitTarget = null;
    private List<GameObject> _vegetablesInrange = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject _closestVegetable = null;
    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
        if (_vegetablesInrange.Count <= 1) return;
        CalculateClosestVegetable();
    }

    public void AddVegetableInRange(GameObject vegetable)
    {
        _vegetablesInrange.Add(vegetable);
        CalculateClosestVegetable();
    }

    public void RemoveVegetableInRange(GameObject vegetable)
    {
        _vegetablesInrange.Remove(vegetable);
        vegetable.GetComponent<MeshRenderer>().material.color = Color.white;
        CalculateClosestVegetable();
    }

    void CalculateClosestVegetable()
    {
        GameObject tempClosestVegetable = null;
        if (_vegetablesInrange.Count == 0)
        {
            _closestVegetable = null;
            return;
        }
        else if(_vegetablesInrange.Count == 1)
        {
            tempClosestVegetable = _vegetablesInrange[0];
        }
        else if(_vegetablesInrange.Count > 1)
        {
            float closestDistanceSqrd = float.MaxValue;
            foreach (var vegetable in _vegetablesInrange)
            {
                Vector3 posDiff = vegetable.transform.position - _hitTarget.transform.position;
                float distanceSqrd = Vector3.SqrMagnitude(posDiff);
                if (distanceSqrd < closestDistanceSqrd)
                {
                    tempClosestVegetable = vegetable;
                    closestDistanceSqrd = distanceSqrd;
                }
            }
        }
        if(tempClosestVegetable != _closestVegetable && _closestVegetable != null) _closestVegetable.GetComponent<MeshRenderer>().material.color = Color.white;
        _closestVegetable = tempClosestVegetable;
        _closestVegetable.GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
