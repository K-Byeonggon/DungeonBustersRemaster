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
        gameLogicManager.OnAllPlayersEnteredDungeon += OnAllPlayersEnteredDungeon;
    }


    public void Exit()
    {
        Debug.Log("Exiting StartDungeon State");
        gameLogicManager.OnAllPlayersEnteredDungeon -= OnAllPlayersEnteredDungeon;
    }

    private void OnAllPlayersEnteredDungeon()
    {
        gameLogicManager.ChangeState(GameState.StartStage);
    }
}
