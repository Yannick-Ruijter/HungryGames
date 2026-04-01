using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public static List<Entity> Entities = new List<Entity>();
    public static int VegetablePlayerCount { get; private set; } = 0;
    public EntityMeshType entityMeshType = EntityMeshType.Farmer;
    public int lives;
    public bool isPlayer;
    

    public UnityEvent onDeath = new UnityEvent();
    
    public UnityEvent<Entity> onEntityReady = new UnityEvent<Entity>();
    
    public UnityEvent<Entity> onDamaged = new UnityEvent<Entity>();

    private void Start()
    {
        Entities.Add(this);
        if (entityMeshType == EntityMeshType.Farmer)
            return;

        if (isPlayer)
            VegetablePlayerCount++;

        entityMeshType = (EntityMeshType)Random.Range(1, 5);
        
        if(!isPlayer) onEntityReady.Invoke(this);
    }

    private void OnDestroy()
    {
        Entities.Remove(this);
    }

    public void AssignType(EntityMeshType type)
    {
        entityMeshType = type;
        onEntityReady.Invoke(this);
    }

    public void TakeDamage()
    {
        lives--;
        if (lives <= 0)
        {
            GameState.Instance.OnDeath(this);
        }
        else
        {
            GameState.Instance.OnDamage(this);
        }
    }
}
