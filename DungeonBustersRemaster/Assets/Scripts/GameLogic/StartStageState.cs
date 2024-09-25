using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStageState : IGameState
{
    private GameLogicManager gameLogicManager;
    public StartStageState(GameLogicManager gameLogicManager)
    {
        this.gameLogicManager = gameLogicManager;
    }

    public void Enter()
    {
        Debug.Log("Entering StartStage State");
    }


    public void Exit()
    {
        Debug.Log("Exiting StartStage State");
    }
}
