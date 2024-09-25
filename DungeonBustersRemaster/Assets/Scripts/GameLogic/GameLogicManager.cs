using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    GameStart,
    DungeonStart,
    StageStart,
    CardSubmission,
    ResultCalculation,
    RewardDistribution,
    StageEnd,
    DungeonEnd,
    GameEnd
}

public class GameLogicManager : NetworkBehaviour
{
    #region Singleton
    private static GameLogicManager _instance;
    public static GameLogicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameLogicManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameLogicManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [SyncVar(hook = nameof(OnPhaseChanged))]
    private GamePhase currentPhase = GamePhase.GameStart;

    private Dictionary<GamePhase, Action> phaseActions;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializePhaseActions();
    }

    private void InitializePhaseActions()
    {
        phaseActions = new Dictionary<GamePhase, Action>
        {
            { GamePhase.GameStart, ExecuteGameStart },
            { GamePhase.DungeonStart, ExecuteDungeonStart },
            { GamePhase.StageStart, ExecuteStageStart },
            { GamePhase.CardSubmission, ExecuteCardSubmission },
            { GamePhase.ResultCalculation, ExecuteResultCalculation },
            { GamePhase.RewardDistribution, ExecuteRewardDistribution },
            { GamePhase.StageEnd, ExecuteStageEnd },
            { GamePhase.DungeonEnd, ExecuteDungeonEnd },
            { GamePhase.GameEnd, ExecuteGameEnd }
        };
    }

    //이거를 적절한 시기에 불러야할듯?
    [Server]
    private void StartGame()
    {
        SetPhase(GamePhase.GameStart);
    }

    //currentPhase는 SyncVar로 모든 클라에 갱신된다.
    [Server]
    private void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        
    }

    [Server]
    private void ExecuteCurrentPhase()
    {
        if(phaseActions.TryGetValue(currentPhase, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogError($"No action defind for phase {currentPhase}");
        }
    }


    #region hook
    private void OnPhaseChanged(GamePhase oldPhase, GamePhase newPhase)
    {
        Debug.Log($"PhaseChanged: {oldPhase} -> {newPhase}");
    }
    #endregion


    #region Phase Action
    private void ExecuteGameStart()
    {
        Debug.Log("Game Start Phase");

    }

    private void ExecuteDungeonStart()
    {
        //이번 던전에서 등장할 몬스터들(4마리) 결정된다.
        //아마 Dictionary<int, Queue<int>> 같은거에 저장되지 않을까? (int = dungeonNum, int = monsterDataId)
    }

    private void ExecuteStageStart()
    {
        //이번 스테이지에 등장할 몬스터가 결정된다. 아마 Dictionary의 Queue에서 하나씩 뽑아오겠지
        //그러면 몬스터는 Monster의 DataId로 뽑아오는게 맞다.
        //모든 몬스터 정보는 MonsterDataManager에 DataId를 Key로, Monster(class)를 Value로 저장하고 있으니까,
        //DataId로 Monster정보를 받아서 Monster(gameScene)의 정보를 갱신해주고, 갱신한걸로 UI 바꾸면 된다.
        //그러면 몬스터 정보를 엄...모든 클라에서 봐야하니까, SyncVar로 하던가 ClienRpc로 클라에 갱신요청하던가 해야함.
        //근데 이런 게임 진행은 Server에서만 관리하니까.. 머리가 아프다

        //다음 몬스터를 향해 달려가는 애니메이션이 재생된다.
    }

    private void ExecuteCardSubmission()
    {
        //모든 플레이어가 카드를 제출할 때까지 대기한다.
    }

    private void ExecuteResultCalculation()
    {
        //플레이어가 제출한 카드의 숫자로 현재 스테이지의 몬스터를 쓰러뜨릴수 있는지 계산한다.
        //플레이어의 공격 성공여부를 갱신해준다.
        //
    }

    private void ExecuteRewardDistribution()
    {
        //승리후 보석분배, 보너스보석 분배 / 패배후 보석회수가 이루어진다.
    }

    private void ExecuteStageEnd()
    {
        //스테이지 4개 다돌았으면 SetPhase(DungeonEnd) 아니면 스테이지 번호 올려서 SetPhase(StageStart)로
    }

    private void ExecuteDungeonEnd()
    {
        //던전 3개 다돌았으면 SetPhase(GameEnd) 아니면 던전 번호 올려서 SetPhase(DungeonStart)로
    }

    private void ExecuteGameEnd()
    {
        //게임결과 보여주고 로비로 돌아갈 수 있는 창을 띄우면 될듯?
    }

    #endregion

}
