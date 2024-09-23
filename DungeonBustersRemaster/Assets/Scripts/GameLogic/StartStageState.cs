using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStageState : MonoBehaviour
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

    public void Update()
    {
        //여기서 조건에 의해 gameLogicManager.ChangeState(GameState.SubmitCardState);
        //그런데 Update로 할게 없어서 이벤트 기반으로 ChageState 할거 같다.

    }

    public void Exit()
    {
        Debug.Log("Exiting StartStage State");
    }
}
