using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    StartDungeon,
    StartStage,
    SubmitCard,
    CalculateResults,
    ShowWinLose,
    GetJewels,
    GetBonus,
    LoseJewels,
    EndStage,
    EndDungeon,
    EndGame
}

public interface IGameState
{
    void Enter();
    void Exit();

}
