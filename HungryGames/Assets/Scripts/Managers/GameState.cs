using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    [SerializeField] private TMP_Text m_CountDownText;

    private Stage m_GameStage = Stage.BeforeStart;
    
    private HashSet<MineController> m_DefusedMines = new HashSet<MineController>();

    public int DefusedMineDifferenceRequirement = 1;
    
    private HashSet<InputDevice> m_DeadDevices = new HashSet<InputDevice>();
    
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
            
        }
        else
        {
            
        }
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
        
        if (MineController.Mines.Count - m_DefusedMines.Count >= DefusedMineDifferenceRequirement)
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
