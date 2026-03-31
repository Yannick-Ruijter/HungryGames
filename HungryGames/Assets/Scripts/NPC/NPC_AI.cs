using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NPC_AI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navAgent;
    //[SerializeField] private NavMeshSurface _navSurface;
    private Vector3 _velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(1, 0, 1), out hit, 1.0f, NavMesh.AllAreas);
        _navAgent.destination = hit.position;
    }

    // Update is called once per frame
    void Update()
    {
        // start of frame, enabl nav agent. get stuff. diable nav agent.
        // con
        _navAgent.enabled = true;
        //get positionn and keep between 0, 1
        //NavMeshHit hit;
        //NavMesh.SamplePosition(new Vector3(1,0,1), out hit, 1.0f, NavMesh.AllAreas);
        //_navAgent.destination = hit.position;
        _velocity = _navAgent.velocity;
        if(_velocity.magnitude > 1)
        {
            _velocity.Normalize();
        }
        
        transform.position += _velocity;
        _navAgent.enabled = false;
    }
}
