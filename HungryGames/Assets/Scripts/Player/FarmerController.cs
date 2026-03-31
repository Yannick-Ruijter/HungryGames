using UnityEngine;
using System.Collections.Generic;
public class FarmerController : PlayerController
{

    private List<GameObject> _vegetablesInrange = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerController.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerController.Update();
    }

    public void AddVegetableInRange(GameObject vegetable)
    {
        _vegetablesInrange.Add(vegetable);
    }

    public void RemoveVegetableInRange(GameObject vegetable)
    {
        _vegetablesInrange.Remove(vegetable);
    }
}
