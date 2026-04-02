using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FarmerVegetableCatcher : MonoBehaviour
{
    [SerializeField] GameObject _hitTarget = null;
    private List<GameObject> _vegetablesInrange = new();
    [SerializeField] private float _stunDelay = 0.5f;

    [SerializeField] private PlayerController m_playerController;

    public UnityEvent onFarmerEatPlayer = new UnityEvent();
    public UnityEvent onAttack = new UnityEvent();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject _closestVegetable = null;

    private GameObject _captured;

    private InputAction _captureAction;

    private void Start()
    {
        _captureAction = GetComponent<PlayerInput>().actions["Interact"];
        _captureAction.performed += CaptureVegetable;
    }

    private void OnDestroy()
    {
        _captureAction.performed -= CaptureVegetable;
    }

    private void Update()
    {
        if (_vegetablesInrange.Count <= 1) return;
        CalculateClosestVegetable();
    }

    private void CaptureVegetable(InputAction.CallbackContext ctx)
    {
        if (_closestVegetable)
        {
            _captured = _closestVegetable;
            _closestVegetable.GetComponent<PlayerController>().CanMove = false;
            onAttack.Invoke();
            Invoke("KillTarget", 0.5f);
            StartCoroutine(Eating());
        }
    }

    private IEnumerator Eating()
    {
        m_playerController.CanMove = false;
        PlayerController controller = _closestVegetable.GetComponent<PlayerController>();
        
        controller.GetComponentInChildren<AnimationPassthrough>().StartAttack();

        if (controller)
            StartCoroutine(ApplyStun(controller));
            
        Entity entity = _closestVegetable.GetComponent<Entity>();
        
        yield return new WaitForSeconds(_stunDelay);
        entity?.TakeDamage();
        if (entity && !entity.isPlayer)
        {
            if (!entity.isPlayer)
            {
                GetComponent<Entity>().TakeDamage();
            }
            else
            {
                onFarmerEatPlayer.Invoke();
            }
        }
        //_vegetablesInrange.Remove(null);
        m_playerController.CanMove = true;
    }

    private IEnumerator ApplyStun(PlayerController controller)
    {
        controller.CanMove = false;
        yield return new WaitForSeconds(_stunDelay);
        controller.CanMove = true;
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
        int nullCounter = 0;
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
                if(vegetable == null)
                {
                    nullCounter++;
                    continue;
                }
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
        for (int i = 0; i < nullCounter; i++) _vegetablesInrange.Remove(null);
    }

    void KillTarget()
    {
        _closestVegetable.GetComponent<PlayerController>().Kill();
    }
}
