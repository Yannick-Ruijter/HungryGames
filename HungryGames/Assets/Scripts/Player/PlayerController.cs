using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private CapsuleCollider m_SolidCollider;
    [SerializeField] private CapsuleCollider m_TriggerCollider;
    
    [SerializeField] private Rigidbody m_RigidBody;

    private InputAction m_MoveInput;
    private InputAction m_JumpInput;
    
    void Start()
    {
        if (!m_PlayerInput)
            m_PlayerInput = GetComponent<PlayerInput>();
        
        m_MoveInput = m_PlayerInput.actions["Move"];
        m_JumpInput = m_PlayerInput.actions["Jump"];
    }
    
    private Vector2 

    // Update is called once per frame
    void Update()
    {
        if (!m_PlayerInput)
            return;
        
        
    }

    private void FixedUpdate()
    {
        if (!m_PlayerInput)
            return;
        
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        
    }

    private void OnCollisionExit(Collision other)
    {
        
    }
}
