using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct ControllerInput
{
    public Vector2 move;
    public bool jump;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private CapsuleCollider m_SolidCollider;
    [SerializeField] private CapsuleCollider m_TriggerCollider;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _rotationSpeed;
    
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private Camera _camera;

    private InputAction m_MoveInput;
    private InputAction m_JumpInput;

    public delegate ControllerInput SampleInputDirection();

    public event SampleInputDirection GiveMeInputElseWhere;
    
    private ControllerInput m_ControllerInput;
    
    private Vector3 m_Direction = Vector3.zero;
    
    private bool m_HasInput
    {
        get { return m_PlayerInput || GiveMeInputElseWhere != null;  }
    }
    
    void Start()
    {
        if (!m_PlayerInput)
            m_PlayerInput = GetComponent<PlayerInput>();
        
        _camera = Camera.main;
        
        m_MoveInput = m_PlayerInput.actions["Move"];
        m_JumpInput = m_PlayerInput.actions["Jump"];
    }

    private Vector2 GetMoveInput()
    {
        if (GiveMeInputElseWhere != null)
            return m_ControllerInput.move;
        return m_MoveInput.ReadValue<Vector2>();
    }

    private bool GetJump()
    {
        if (GiveMeInputElseWhere != null)
            return m_ControllerInput.jump;
        return m_JumpInput.IsPressed();
    }

    // Update is called once per frame
    void Update()
    {
        if (GiveMeInputElseWhere != null)
        {
            m_ControllerInput = GiveMeInputElseWhere.Invoke();
        }
        if (!m_HasInput)
            return;
        
        transform.forward = Vector3.Lerp(transform.forward, m_Direction, Time.deltaTime * 5.0f / Mathf.Max(_rotationSpeed, 0.01f));
    }

    private void FixedUpdate()
    {
        if (!m_PlayerInput)
            return;
        
        Vector2 moveInput = GetMoveInput();
        bool isJumping = GetJump();
        
        HandleDirection();
        
        float moveInputV = Mathf.Clamp01(Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y));
        
        Vector3 velocity = Vector3.Lerp(
            m_RigidBody.linearVelocity,
            transform.forward * (_movementSpeed * moveInputV),
            Time.fixedDeltaTime * _acceleration
        );

        velocity.y = m_RigidBody.linearVelocity.y;

        m_RigidBody.linearVelocity = velocity;
    }

    private void HandleDirection()
    {
        Vector3 cameraForward = _camera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _camera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector2 moveInput = GetMoveInput();
        if (GiveMeInputElseWhere == null)
            m_Direction = cameraForward * moveInput.y + cameraRight * moveInput.x;
        else
            m_Direction = moveInput;
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
