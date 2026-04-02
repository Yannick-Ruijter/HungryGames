using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPassthrough : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private PlayerController m_playerController;
    private EntityAnimationController m_entityAnimationController;

    [SerializeField] private float m_AttackDuration = 0.5f;
    [SerializeField] private float m_JumpDuration = 0.5f;

    [SerializeField] private EntityMeshAnimationPair[] m_meshPairs;

    private bool m_isInAir = false;
    private bool m_isAttacking = false;

    public void onReady(Entity ent)
    {
        foreach (var pair in m_meshPairs)
        {
            if (pair.type == entity.entityMeshType)
            {
                m_entityAnimationController = pair.controller;
            }
        }
    }

    private void OnEnable()
    {
        m_isInAir = false;
        m_isAttacking = false;
    }

    private void Update()
    {
        ControllerInput input = m_playerController.GetOutputInput();
        bool grounded = m_playerController.IsGrounded();

        if (m_isAttacking)
            return;

        if (!m_isInAir)
        {
            if (grounded && input.jump)
            {
                m_entityAnimationController.type = EntityAnimationController.AnimationType.jump;
                StartCoroutine(PerformJump());
                return;
            }

            float v = Mathf.Clamp01(Mathf.Abs(input.move.x) + Mathf.Abs(input.move.y));

            if (v > 0.5f)
            {
                m_entityAnimationController.type = EntityAnimationController.AnimationType.walk;
            }
            else
            {
                m_entityAnimationController.type = EntityAnimationController.AnimationType.idle;
            }
        }
    }

    private IEnumerator PerformJump()
    {
        m_isInAir = true;
        yield return new WaitForSeconds(m_JumpDuration);
        m_isInAir = false;
    }

    public void StartAttack()
    {
        m_entityAnimationController.type = EntityAnimationController.AnimationType.attack;
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        m_isAttacking = true;
        yield return new WaitForSeconds(m_AttackDuration);
        m_isAttacking = false;
    }

    [System.Serializable]
    public struct EntityMeshAnimationPair
    {
        public EntityMeshType type;
        public EntityAnimationController controller;
    }
}
