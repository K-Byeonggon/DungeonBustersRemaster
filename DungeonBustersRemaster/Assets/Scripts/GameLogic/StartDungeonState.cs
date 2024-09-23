using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDungeonState : IGameState
{
    private GameLogicManager gameLogicManager;

    public StartDungeonState(GameLogicManager gameLogicManager)
    {
        this.gameLogicManager = gameLogicManager;
    }

    public void Enter()
    {
        Debug.Log("Entering StartDungeon State");
    }

    public void Update()
    {
        //
    }

    public void Exit()
    {
        Debug.Log("Exiting StartDungeon State");
    }

}
