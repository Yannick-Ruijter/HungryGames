using System;
using UnityEngine;

public class AnimationPassthrough : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private PlayerController m_playerController;
    private EntityAnimationController m_entityAnimationController;

    [SerializeField] private EntityMeshAnimationPair[] m_meshPairs;

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

    private void Update()
    {
        
    }

    [System.Serializable]
    public struct EntityMeshAnimationPair
    {
        public EntityMeshType type;
        public EntityAnimationController controller;
    }
}
