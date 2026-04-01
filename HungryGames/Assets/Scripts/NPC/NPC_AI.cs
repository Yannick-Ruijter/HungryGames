using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public float _wanderRadius;
    [SerializeField] public float _wanderAngleChange;
    [SerializeField] public float _diffuseTime = 5f;
    [SerializeField] public float _fieldRadius = 10;
    [SerializeField] public float _borderBuffer = 1;

    private float _wanderAngle = 0;
    float tiimer;
    private state _state = state.wandering;
    private bool _toBomb = false;

    private List<MineController> _bombs;
    private targetStruct _closestBomb = new targetStruct();
    private float _stateTime;

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
        Debug.Log(_bombs);
        _playerController.GiveMeInputElseWhere += GetControllerInput;
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(1, 0, 1), out hit, 100.0f, NavMesh.AllAreas);
        _navAgent.destination = hit.position;
        _navAgent.updatePosition = false;
        _stateTime = Random.Range(0, 5.0f);

        //_navAgent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _navAgent.nextPosition = transform.position;

        tiimer -= Time.deltaTime;

        if (tiimer < 0)
        {
            if(_canJump) _jump = true;
            tiimer = 1f;
            ChangeDirction();
        }

        // seek 20 percent towards a bomb, stand still, repeat

        switch(_state)
        {
            case state.wandering:
                {
                    _stateTime -= Time.deltaTime;
                    if (_stateTime <= 0)
                    {
                        if(Random.Range(0f, 5f) > 1)
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
                    break;
                }
            case state.defusing:
                {
                    _controllerInput.move = new Vector2();
                    _controllerInput.jump = false;
                    _stateTime -= Time.deltaTime;
                    if(_stateTime <= 0)
                    {
                        _stateTime = Random.Range(1f, _diffuseTime);
                        _state = state.wandering;
                    }
                    break;
                }
            case state.standing:
                {
                    // same as diffusing but might be diferent due to animations
                    _controllerInput.move = new Vector2();
                    _controllerInput.jump = false;
                    _stateTime -= Time.deltaTime;
                    if (_stateTime <= 0)
                    {
                        _stateTime = Random.Range(1f, _diffuseTime);
                        _state = state.wandering;
                    }
                    break;
                }
            case state.searching:
                {
                    if (_navAgent.remainingDistance <= 2f)
                    {
                        if (_toBomb)
                            _state = state.defusing;
                        else _state = state.standing;
                            _stateTime = Random.Range(0, 10.0f);
                    }
                    _controllerInput.move = GetCombinedDir();
                    _controllerInput.jump = _jump;
                    break;
                }
        }
            
        
    }

    private void FixedUpdate()
    {
        _canJump = true;
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

        if(transform.position.magnitude > _fieldRadius - _borderBuffer)
        {
            Debug.Log("border");
            dir += -transform.position.normalized * 2f;
        }


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
        Vector3 randomDirection = Random.insideUnitSphere * _fieldRadius;
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
