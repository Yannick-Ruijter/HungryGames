using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Shaders;



public class NPC_AI : MonoBehaviour
{
    public struct targetStruct
    {
        public float distace;
        public GameObject obj;
    }

    enum state
    {
        defusing,
        wandering,
        searching,
        standing,
    }

    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] public GameObject _bombPrefab;
    [Header("bounds")]
    [SerializeField] public float _fieldRadius = 10;
    [SerializeField] public float _borderBuffer = 1;

    [Header("behaviour")]
    [SerializeField] public float _minswitchTime = 0;
    [SerializeField] public float _maxswitchTime = 0;
    [Header("jumping")]
    [SerializeField] public float _minJumpTime = 0;
    [SerializeField] public float _maxJumpTime = 3;
    [Header("wander")]
    [SerializeField] public float _wanderRadius;
    [SerializeField] public float _wanderAngleChange;
    [SerializeField] public float _minwanderTime = 0f;
    [SerializeField] public float _maxwanderTime = 5f;
    [Header("diffusing")]
    [SerializeField] public float _maxDiffuseTime = 5f;
    [SerializeField] public int _maxDiffuseChance = 5;

    

    private float _wanderAngle = 0;
    float _changeBehaviourTimer;
    private state _state = state.wandering;
    private bool _toBomb = false;

    private List<MineController> _bombs;
    private targetStruct _closestBomb = new targetStruct();
    private float _stateTime;

    private bool _jump;
    private bool _canJump;
    private float _jumpTimer;

    private bool _isDefusing;
    private bool _wasDefusing;

    private Vector3 _velocity;
    private ControllerInput _controllerInput;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _changeBehaviourTimer = 1f;
        _bombs = FindAllPrefabInstances();
        Debug.Log(_bombs);
        _playerController.GiveMeInputElseWhere += GetControllerInput;
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(1, 0, 1), out hit, 100.0f, NavMesh.AllAreas);
        _navAgent.destination = hit.position;
        _navAgent.updatePosition = false;
        _stateTime = UnityEngine.Random.Range(0, 5.0f);
        _jumpTimer = UnityEngine.Random.Range(_minJumpTime, _maxJumpTime);

        //_navAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _isDefusing = false;
        _navAgent.nextPosition = transform.position;

        _changeBehaviourTimer -= Time.deltaTime;

        if (_changeBehaviourTimer < 0)
        {
            _changeBehaviourTimer = UnityEngine.Random.Range(_minswitchTime, _maxswitchTime);
            _state = (state)UnityEngine.Random.Range(0f, 5);
        }
        _jumpTimer -= Time.deltaTime;
        if (_jumpTimer <= 0)
        {
            if (_canJump) _jump = true;
            _jumpTimer = UnityEngine.Random.Range(_minJumpTime, _maxJumpTime);
        }

        // seek 20 percent towards a bomb, stand still, repeat


        switch (_state)
        {
            case state.wandering:
                {
                    WanderBehaviour();
                    break;
                }
            case state.defusing:
                {
                    DiffuseBehaviour();
                    break;
                }
            case state.standing:
                {
                    StandingBehaviour();
                    break;
                }
            case state.searching:
                {
                    SearchBehaviour();
                    break;
                }
        }

        if (transform.position.magnitude > _fieldRadius - _borderBuffer)
        {
            _controllerInput.move = new Vector2(-transform.position.normalized.x, -transform.position.normalized.z) ;

        }

        if(_isDefusing && !_wasDefusing)
        _controllerInput.diffuseState = ControllerInput.DiffuseState.start;
        if (_isDefusing && _wasDefusing)
            _controllerInput.diffuseState = ControllerInput.DiffuseState.doing;
        if (!_isDefusing && _wasDefusing)
            _controllerInput.diffuseState = ControllerInput.DiffuseState.end;
        if (!_isDefusing && !_wasDefusing)
            _controllerInput.diffuseState = ControllerInput.DiffuseState.not;


        _wasDefusing = _isDefusing;

    }

    public void SwitchToSearch()
    {
        if (UnityEngine.Random.Range(0f, 5f) > 1)
        {
            _navAgent.destination = RandomNavMeshLocation();
            _toBomb = false;
        }
        else
        {
            CalcClosestBomb();
            _navAgent.destination = _closestBomb.obj.transform.position;
            _toBomb = true;
        }
        _state = state.searching;
    }
    public void SearchBehaviour()
    {
        if (_navAgent.remainingDistance <= 2f)
        {
            SwitchToStation();
        }
        _controllerInput.move = GetCombinedDir();
        _controllerInput.jump = _jump;
    }

    void SwitchToStation()
    {
        int min = 0;
        int max = _maxDiffuseChance;
        if (_toBomb)
        {
            max = _maxDiffuseChance - 2;
        }
        if (UnityEngine.Random.Range(min, max) > 1)
        {
            _state = state.standing;
        }
        else
        {
            
            _state = state.defusing;
        }
        _stateTime = UnityEngine.Random.Range(0, _maxDiffuseTime);
    }

    public void DiffuseBehaviour()
    {
        _isDefusing = true;
        _controllerInput.move = new Vector2();
        _controllerInput.jump = false;
        _stateTime -= Time.deltaTime;
        if (_stateTime <= 0)
        {
            SwitchToWander();
        }
    }

    public void StandingBehaviour()
    {
        // same as diffusing but might be diferent due to animations
        _controllerInput.move = new Vector2();
        _controllerInput.jump = false;
        _stateTime -= Time.deltaTime;
        if (_stateTime <= 0)
        {
            SwitchToWander();
        }
    }

    void SwitchToWander()
    {
        
        _stateTime = UnityEngine.Random.Range(_minwanderTime, _maxwanderTime);
        _state = state.wandering;
    }

    public void WanderBehaviour()
    {
        _stateTime -= Time.deltaTime;
        if (_stateTime <= 0)
        {
            if (UnityEngine.Random.Range(0f, 5f) > 1)
            {
                _navAgent.destination = RandomNavMeshLocation();
                _toBomb = false;
            }
            else
            {
                CalcClosestBomb();
                _navAgent.destination = _closestBomb.obj.transform.position;
                _toBomb = true;
            }
            _state = state.searching;
        }
        _controllerInput.move = GetWanderDir();
        _controllerInput.jump = _jump;
    }

    private void FixedUpdate()
    {
        _canJump = true;
        _jump = false;
    }



    private ControllerInput GetControllerInput()
    {
        return _controllerInput;
    }

    private List<MineController> FindAllPrefabInstances()
    {
        return FindObjectsByType<MineController>(FindObjectsSortMode.None).ToList();
        //return PrefabUtility.FindAllInstancesOfPrefab(_bombPrefab, SceneManager.GetActiveScene());
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
        _wanderAngle += UnityEngine.Random.Range(-_wanderAngleChange, _wanderAngleChange);

        //Debug.Log(Mathf.Rad2Deg * _wanderAngle);

        if (_wanderAngle > 180)// Mathf.PI)
        {
            _wanderAngle -= 2f * Mathf.PI;
        }
        if (_wanderAngle < -180)//-Mathf.PI)
        {
            _wanderAngle += 2f * Mathf.PI;
        }
    }
    /**/
    private Vector2 GetWanderDir()
    {
        ChangeDirction();
        Quaternion quat = Quaternion.Euler(0f, _wanderAngle, 0f);

        
        //Debug.Log(_wanderAngle);
        Vector3 dir =  quat * transform.forward ;


        


        dir.Normalize();

        //Debug.Log(dir);

        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, dir, Color.red);

        //return new Vector2(0, 1);
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
        foreach(var bomb in _bombs)
        {
            Debug.Log("testing bomb");
            float distance = Vector3.Distance(transform.position, bomb.transform.position);
            if (closestDistance > distance || closestDistance < 0)
            {
                closest = bomb.gameObject;
                closestDistance = distance;
            }
        }
        _closestBomb.obj = closest;
        _closestBomb.distace = closestDistance;
    }

    public Vector3 RandomNavMeshLocation()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _fieldRadius;
        //randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, _fieldRadius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

}
