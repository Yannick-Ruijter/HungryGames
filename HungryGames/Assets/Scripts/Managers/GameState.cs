using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [SerializeField] private TMP_Text m_CountDownText;

    private Stage m_GameStage = Stage.BeforeStart;

    [Tooltip("Round time in seconds.")]
    public float RoundTime = 300.0f;
    
    private HashSet<MineController> m_DefusedMines = new HashSet<MineController>();

    public int DefusedMineDifferenceRequirement = 1;

    private int DeadPlayerCount = 0;
    
    private bool IsGameFinished
    {
        get { return m_GameStage == Stage.FarmerWin || m_GameStage == Stage.VegetablesWin; }
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        m_GameStage = Stage.StartSequence;
        
        foreach (PlayerController controller in PlayerController.Controllers)
            controller.CanMove = false;

        for (int countDownIndex = 0; countDownIndex < 3; countDownIndex++)
        {
            int time = 3 - countDownIndex;
            
            m_CountDownText.text = time.ToString();
            
            yield return new WaitForSeconds(1.0f);
        }
        
        // Begin game for everything

        m_CountDownText.text = "START!!!";

        foreach (PlayerController controller in PlayerController.Controllers)
            controller.CanMove = true;
        
        m_GameStage = Stage.Started;
    }

    public void OnDeath(Entity entity)
    {
        if (IsGameFinished)
            return;
        entity.onDeath.Invoke();
        if (entity.isPlayer)
        {
            if (entity.entityMeshType == EntityMeshType.Farmer)
            {
                VegetablesWin();
            }
            else
            {
                DeadPlayerCount++;
                if (Entity.VegetablePlayerCount - DeadPlayerCount == 0)
                {
                    // All players are dead
                    
                    FarmerWins();
                }
            }
        }
    }

    private void GetTime(out float minutes, out float seconds)
    {
        float time = RoundTime - Time.time;

        minutes = Mathf.Floor(time / 60.0f);
        seconds = Mathf.Floor(time % 60.0f);
    }

    public string FormattedTime()
    {
        float minutes, seconds;
        GetTime(out minutes, out seconds);

        return $"{minutes::0}:{seconds::00}";
    }

    public void OnDamage(Entity entity)
    {
        if (IsGameFinished)
            return;
        if (!entity.isPlayer)
        {
            OnDeath(entity);
            return;
        }

        if (entity.entityMeshType == EntityMeshType.Farmer)
            return;

        List<Entity> npc_veg =
            Entity.Entities.FindAll((entity) => !entity.isPlayer && entity.entityMeshType != EntityMeshType.Farmer);

        if (npc_veg.Count == 0)
        {
            Debug.LogError("No npc vegetables found");
            return;
        }

        Entity target = npc_veg[Random.Range(0, npc_veg.Count)];

        entity.entityMeshType = target.entityMeshType;
        
        entity.transform.position = target.transform.position;
        entity.transform.rotation = target.transform.rotation;
        Destroy(target.gameObject);
        
        entity.onEntityReady.Invoke(entity);
    }

    private void VegetablesWin()
    {
        m_GameStage = Stage.VegetablesWin;
    }

    private void FarmerWins()
    {
        m_GameStage = Stage.FarmerWin;
    }

    public void OnMineDefuse(MineController mine)
    {
        if (IsGameFinished)
            return;
        m_DefusedMines.Add(mine);
        
        if (MineController.Mines.Count - m_DefusedMines.Count <= DefusedMineDifferenceRequirement)
        {
            VegetablesWin();
        }
    }

    private enum Stage
    {
        BeforeStart,
        StartSequence,
        Started,
        FarmerWin,
        VegetablesWin
    }
}
