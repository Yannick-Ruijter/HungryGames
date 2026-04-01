using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public EntityMeshType entityMeshType = EntityMeshType.Farmer;
    public int lives;
    public bool isPlayer;

    public UnityEvent onDeath = new UnityEvent();

    private void Start()
    {
        if (entityMeshType == EntityMeshType.Farmer)
            return;

        entityMeshType = (EntityMeshType)Random.Range(1, 5);
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
