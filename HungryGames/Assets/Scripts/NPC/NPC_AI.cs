using NUnit.Framework;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NPC_AI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] public GameObject _bombPrefab;
    private GameObject[] _bombs;

    private Vector3 _velocity;
    private ControllerInput _controllerInput;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bombs = FindAllPrefabInstances();
        _playerController.GiveMeInputElseWhere += GetControllerInput;
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(1, 0, 1), out hit, 100.0f, NavMesh.AllAreas);
        _navAgent.destination = hit.position;
        _navAgent.updatePosition = false;
        //_navAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // start of frame, enabl nav agent. get stuff. diable nav agent.
        // con
        //_navAgent.enabled = true;
        //get positionn and keep between 0, 1
        _navAgent.nextPosition = transform.position;
        _velocity = _navAgent.velocity;
        if(_velocity.magnitude > 1)
        {
            _velocity.Normalize();
        }

        if(_navAgent.remainingDistance <= 2f)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f)), out hit, 100.0f, NavMesh.AllAreas);
            _navAgent.destination = hit.position;
        }


        _controllerInput.move = new Vector2(_velocity.x, _velocity.z);
        _controllerInput.jump = false;

        //_navAgent.enabled = false;
    }

    private ControllerInput GetControllerInput()
    {
        return _controllerInput;
    }

    private GameObject[] FindAllPrefabInstances()
    {
        return PrefabUtility.FindAllInstancesOfPrefab(_bombPrefab, SceneManager.GetActiveScene());
    }

    private GameObject closestBomb()
    {
        return null;
    }

}
