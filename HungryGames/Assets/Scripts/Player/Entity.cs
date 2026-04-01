using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public EntityMeshType entityMeshType = EntityMeshType.Farmer;
    public int lives;
    public bool isPlayer;

    public UnityEvent onDeath = new UnityEvent();
    
    public UnityEvent<Entity> onEntityReady = new UnityEvent<Entity>();

    private void Start()
    {
        if (entityMeshType == EntityMeshType.Farmer)
            return;

        entityMeshType = (EntityMeshType)Random.Range(1, 5);
        
        onEntityReady.Invoke(this);
    }

    private void OnDestroy()
    {
        
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
