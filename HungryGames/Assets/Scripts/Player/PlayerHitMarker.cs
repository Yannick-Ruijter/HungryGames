using UnityEngine;

public class PlayerHitMarker : MonoBehaviour
{
    private FarmerController _farmer = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _farmer = GetComponentInParent<FarmerController>();
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
