using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct ControllerInput
{
    public enum DiffuseState
    {
        start,
        doing,
        end,
        not
    }
    public Vector2 move;
    public bool jump;
    public DiffuseState diffuseState;
}

public class PlayerController : MonoBehaviour
{
    public static List<PlayerController> Controllers { get; private set; } = new List<PlayerController>();
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private CapsuleCollider m_SolidCollider;
    [SerializeField] private CapsuleCollider m_TriggerCollider;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _impulseJumpVelocity = 2.0f;
    
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private Camera _camera;

    private InputAction m_MoveInput;
    private InputAction m_JumpInput;

    public delegate ControllerInput SampleInputDirection();

    public event SampleInputDirection GiveMeInputElseWhere;
    
    public ControllerInput m_ControllerInput;
    
    private Vector3 m_Direction = Vector3.zero;

    public bool m_IsGrounded = false;

    public bool CanMove = true;

    private bool inputAssigned = false;
    
    private bool m_HasInput
    {
        get { return m_PlayerInput || GiveMeInputElseWhere != null;  }
    }
    
    void Start()
    {
        Controllers.Add(this);
        if (!m_PlayerInput)
            m_PlayerInput = GetComponent<PlayerInput>();
        
        _camera = Camera.main;
        
        if (!inputAssigned && m_PlayerInput.actions != null)
        {
            m_MoveInput = m_PlayerInput.actions["Move"];
            m_JumpInput = m_PlayerInput.actions["Jump"];
            inputAssigned = true;
        }
    }

    private void OnDestroy()
    {
        Controllers.Remove(this);
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
        {
            return m_ControllerInput.jump;
        }

        return m_JumpInput.IsPressed();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!inputAssigned && m_PlayerInput.actions != null)
        {
            m_MoveInput = m_PlayerInput.actions["Move"];
            m_JumpInput = m_PlayerInput.actions["Jump"];
            inputAssigned = true;
        }

        if (!CanMove)
            return;
        if (GiveMeInputElseWhere != null)
        {
            m_ControllerInput = GiveMeInputElseWhere.Invoke();
        }
        if (!m_HasInput)
            return;

        m_Direction.y = 0.0f;
        
        transform.forward = Vector3.Lerp(transform.forward, m_Direction, Time.deltaTime * 5.0f / Mathf.Max(_rotationSpeed, 0.01f));
    }

    private void FixedUpdate()
    {
        if (!CanMove)
            return;
        if (!m_HasInput)
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
        
        if (m_IsGrounded && isJumping)
        {
            m_IsGrounded = false;

            velocity.y += _impulseJumpVelocity;
        }

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
        PlayerController otherController = other.GetComponent<PlayerController>();
        if (otherController)
        {
            Vector3 p31 = transform.position;
            Vector3 p32 = otherController.transform.position;

            Vector2 p21 = new Vector2(p31.x, p31.z), p22 = new Vector2(p32.x, p32.z);
            
            float distance = Vector2.Distance(p21, p22);

            float totalRadius = m_TriggerCollider.radius + otherController.m_TriggerCollider.radius;
            
            float interp = Mathf.Max((totalRadius - distance) / totalRadius, 0.1f);

            Vector2 direction = distance != 0.0f ? (p21 - p22) / distance : new Vector2(0.0f, 0.0f);

            m_RigidBody.linearVelocity += new Vector3(direction.x, 0.0f, direction.y) * (interp * Time.fixedDeltaTime) * 30.0f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.8f)
            {
                m_IsGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.8f)
            {
                m_IsGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        m_IsGrounded = false;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
