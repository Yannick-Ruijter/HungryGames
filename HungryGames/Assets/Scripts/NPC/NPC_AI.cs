using NUnit.Framework;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;



public class NPC_AI : MonoBehaviour
{
    public struct targetStruct
    {
        public float distace;
        public GameObject obj;
    }

    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] public GameObject _bombPrefab;
    [SerializeField] public float _wanderRadius;
    [SerializeField] public float _wanderAngleChange;

    private float _wanderAngle = 0;
    float tiimer;

    private GameObject[] _bombs;
    private targetStruct _closestBomb = new targetStruct();

    private bool _jump;
    private bool _canJump;

    private Vector3 _velocity;
    private ControllerInput _controllerInput;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiimer = 1f;
        _wanderAngleChange = Mathf.Deg2Rad * _wanderAngleChange;
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
        _navAgent.nextPosition = transform.position;

        if (_navAgent.remainingDistance <= 2f)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f)), out hit, 100.0f, NavMesh.AllAreas);
            _navAgent.destination = hit.position;
        }
        tiimer -= Time.deltaTime;
        if (tiimer < 0)
        {
            if(_canJump) _jump = true;
            tiimer = 1f;
            ChangeDirction();
        }


        _controllerInput.move = GetCombinedDir();
        _controllerInput.jump = _jump;
    }

    private void FixedUpdate()
    {
        _canJump = true;
    }



    private ControllerInput GetControllerInput()
    {
        return _controllerInput;
    }

    private GameObject[] FindAllPrefabInstances()
    {
        return PrefabUtility.FindAllInstancesOfPrefab(_bombPrefab, SceneManager.GetActiveScene());
    }

    private Vector2 GetCombinedDir()
    {
        return ((GetNavDir() + GetWanderDir()) / 2).normalized;
    }

    private Vector2 GetNavDir()
    {
        return new Vector2(_navAgent.velocity.x, _navAgent.velocity.z);
    }

    private void ChangeDirction()
    {
        _wanderAngle += Random.Range(-_wanderAngleChange, _wanderAngleChange);

        //Debug.Log(Mathf.Rad2Deg * _wanderAngle);

        if (_wanderAngle > Mathf.PI)
        {
            _wanderAngle -= 2f * Mathf.PI;
        }
        if (_wanderAngle < -Mathf.PI)
        {
            _wanderAngle += 2f * Mathf.PI;
        }
    }

    private Vector2 GetWanderDir()
    {
        Quaternion quat = Quaternion.Euler(0f, Mathf.Rad2Deg * _wanderAngle, 0f);

        
        //Debug.Log(_wanderAngle);
        Vector3 dir =  quat * Vector3.forward ;
        //Debug.DrawRay(transform.position, transform.forward, Color.blue);
        //Debug.DrawRay(transform.position, dir, Color.red);
        dir.Normalize();
        return new Vector2(dir.x, dir.z);
        
        //Vector2 direction = new Vector2(transform.forward.x, transform.forward.z);
        //Vector2 circleMid = direction * _wanderRadius;

        //Vector2 circlePoint = new Vector2(Mathf.Cos(_wanderAngle),Mathf.Sin(_wanderAngle)) * _wanderRadius;

        //Vector2 outDirection = circleMid + circlePoint;
        
        ////outDirection.Normalize();

        //return outDirection;
    }

    private void CalcClosestBomb()
    {
        float closestDistance = -1;
        GameObject closest = null;
        foreach(GameObject bomb in _bombs)
        {
            float distance = Vector3.Distance(transform.position, bomb.transform.position);
            if (closestDistance > distance || closestDistance < 0)
            {
                closest = bomb;
                closestDistance = distance;
            }
        }
        _closestBomb.obj = closest;
        _closestBomb.distace = closestDistance;
    }

}
