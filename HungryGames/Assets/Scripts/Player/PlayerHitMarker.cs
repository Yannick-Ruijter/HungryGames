using UnityEngine;

public class PlayerHitMarker : MonoBehaviour
{
    private FarmerVegetableCatcher _farmer = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _farmer = GetComponentInParent<FarmerVegetableCatcher>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vegetable"))
        {
            _farmer.AddVegetableInRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vegetable"))
        {
            _farmer.RemoveVegetableInRange(other.gameObject);
        }
    }
}
