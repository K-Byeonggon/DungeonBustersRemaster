using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
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
    //NetworkBehaviour에 Singleton패턴을 적용하면 NetworkManager에 의한 관리와 충돌할 수 있다.
    public static GameLogicManager Instance;

    [SerializeField] MonsterSpawner monsterSpawner;

    private int gamePlayerCount;

    [SyncVar(hook = nameof(OnPhaseChanged))]
    private GamePhase currentPhase = GamePhase.GameStart;

    private Dictionary<GamePhase, Action> phaseActions;

    [SyncVar(hook = nameof(OnCurrentDungeonChanged))]
    private int currentDungeon;

    private Queue<int> currentDungeonMonsterIds = new Queue<int>();

    [SyncVar(hook = nameof(OnCurrentStageChanged))]
    private int currentStage;

    [SyncVar(hook = nameof(OnCurrentMonsterDataIdChanged))]
    private int currentMonsterDataId;

    [SyncVar(hook = nameof(OnSumittedPlayerCountChanged))]
    private int submittedPlayerCount;

    private readonly SyncList<int> bonusGems = new SyncList<int>();

    public List<int> BonusGems => bonusGems.ToList<int>();

    
    public override void OnStartServer()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GameLogicManagers in the scene.");
        }
    }
    
    public override void OnStartClient()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GameLogicManagers in the scene.");
        }
    }

    private void Awake()
    {
        MyNetworkRoomManager.Instance.OnAllGamePlayerLoaded += StartGame;

        bonusGems.OnChange += OnBonusGemsChanged;



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
    private void StartGame(int gamePlayerCount)
    {
        this.gamePlayerCount = gamePlayerCount;
        Debug.Log(this.gamePlayerCount + "명의 GamePlayer");
        SetPhase(GamePhase.GameStart);
    }

    //currentPhase는 SyncVar로 모든 클라에 갱신된다.
    [Server]
    private void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        ExecuteCurrentPhase();
    }

    [Server]
    private void ExecuteCurrentPhase()
    {
        if (phaseActions.TryGetValue(currentPhase, out Action action))
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

    private void OnCurrentDungeonChanged(int oldDungeon, int newDungeon)
    {
        Debug.Log($"DungeonChanged: {oldDungeon} -> {newDungeon}");
        UI_StageInfo.UpdateDungeonInfo(newDungeon);
    }

    private void OnCurrentStageChanged(int oldStage, int newStage)
    {
        Debug.Log($"StageChanged: {oldStage} -> {newStage}");
        UI_StageInfo.UpdateStageInfo(newStage);
    }

    private void OnCurrentMonsterDataIdChanged(int oldMonsterId, int newMonsterId)
    {
        Debug.Log($"MonsterIdChanged: {oldMonsterId} -> {newMonsterId}");
        UI_MonsterInfo.UpdateMonsterInfo(newMonsterId);
    }

    private void OnBonusGemsChanged(SyncList<int>.Operation op, int oldItem, int newItem)
    {
        switch(op)
        {
            case SyncList<int>.Operation.OP_ADD:
                UI_BonusGems.UpdateBonusGems(BonusGems);
                break;
            case SyncList<int>.Operation.OP_SET:
                UI_BonusGems.UpdateBonusGems(BonusGems);
                break;
        }
    }

    private void OnSumittedPlayerCountChanged(int oldCount, int newCount)
    {
        Debug.Log($"SubmittedPlayerCountChanged: {oldCount} -> {newCount}");
        ServerCheckSubmittedPlayerCount();
    }
    #endregion


    #region Phase Action
    private void ExecuteGameStart()
    {
        Debug.Log("Game Start Phase");


        InitializeGame();

        SetPhase(GamePhase.DungeonStart);
    }

    private void InitializeGame()
    {
        currentDungeon = 0;
        currentStage = 0;
        currentDungeonMonsterIds.Clear();
        bonusGems.AddRange(new List<int>() { 0, 0, 0 });

    }

    private void ExecuteDungeonStart()
    {
        //이번 던전에서 등장할 몬스터들(4마리) 결정된다.
        //아마 Dictionary<int, Queue<int>> 같은거에 저장되지 않을까? (int = dungeonNum, int = monsterDataId)
        currentDungeon++;
        List<int> monsterIdList = MonsterDataManager.Instance.GetRandomMonsterDataIdsByDungeon(currentDungeon);
        currentDungeonMonsterIds = new Queue<int>(monsterIdList);

        SetPhase(GamePhase.StageStart);
    }

    #region StageStart

    private void ExecuteStageStart()
    {
        //이번 스테이지에 등장할 몬스터가 결정된다.

        //다음 몬스터를 향해 달려가는 애니메이션이 재생된다.

        //이거는 SyncVar라서 클라에도 알아서 변경됨
        currentStage++;
        currentMonsterDataId = currentDungeonMonsterIds.Dequeue();

        //currentMonsterDataId가 SyncVar지만, 동기화 지연때문에 값을 직접 전달해야함. 아니면 이것도 hook함수로 처리하든지.
        RpcMonsterSpawnOnStageStart(currentMonsterDataId);

        //몬스터 생성하면서 UI도 다시띄워준다.
        RpcUIChangeOnStageStart();

        SetPhase(GamePhase.CardSubmission);
    }

    [ClientRpc]
    private void RpcMonsterSpawnOnStageStart(int monsterDataId)
    {
        monsterSpawner.SpawnMonster(monsterDataId);
    }

    [ClientRpc]
    private void RpcUIChangeOnStageStart()
    {
        UIManager.Instance.ShowUI(UIPrefab.MonsterInfoUI);
        UIManager.Instance.ShowUI(UIPrefab.BonusGemsUI);
        UIManager.Instance.ShowUI(UIPrefab.TimerUI);
        UIManager.Instance.ShowUI(UIPrefab.CardPanelUI);
        UIManager.Instance.ShowUI(UIPrefab.PlayerInfoUI);
    }

    #endregion

    #region CardSubmission

    private void ExecuteCardSubmission()
    {
        //모든 플레이어가 카드를 제출할 때까지 대기한다.
    }

    [Command(requiresAuthority = false)]
    public void CmdAddSubmittedPlayerCount()
    {
        submittedPlayerCount++;
    }
    [Command(requiresAuthority = false)]
    public void CmdSubSubmittedPlayerCount()
    {
        submittedPlayerCount--;
    }

    private void ServerCheckSubmittedPlayerCount()
    {
        if (submittedPlayerCount == gamePlayerCount)
        {
            SetPhase(GamePhase.ResultCalculation);
        }
    }

    #endregion

    private void ExecuteResultCalculation()
    {
        //플레이어가 제출한 카드의 숫자로 현재 스테이지의 몬스터를 쓰러뜨릴수 있는지 계산한다.
        //플레이어의 공격 성공여부를 갱신해준다.
        ResultCalculator calculator = new ResultCalculator();
        calculator.SetSubmittedCardNums();
        int sumOfAttack = calculator.GetSumOfAttack();
        int monsterHP = MonsterDataManager.Instance.LoadedMonsters[currentMonsterDataId].HP;

        Debug.Log($"sumOfAttack: {sumOfAttack}  HP: {monsterHP}");
        if (sumOfAttack >= monsterHP)
        {
            //이김
            Debug.Log("You Win");
        }
        else
        {
            //짐
            Debug.Log("You Lose");
        }
    }



    private void ExecuteRewardDistribution()
    {
        //승리후 보석분배, 보너스보석 분배 / 패배후 보석회수가 이루어진다.
    }

    [Command(requiresAuthority = false)]
    public void CmdSubBonusGems(GemColor color, int subAmount)
    {
        bonusGems[(int)color] -= subAmount;
    }

    private void ExecuteStageEnd()
    {
        //아직 몬스터 남았으면 StageStart로
        if (currentDungeonMonsterIds.Count > 0)
        {
            SetPhase(GamePhase.StageStart);
        }
        else
        {
            SetPhase(GamePhase.DungeonEnd);
        }
    }

    private void ExecuteDungeonEnd()
    {
        //던전 3개 다돌았으면 SetPhase(GameEnd) 아니면 던전 번호 올려서 SetPhase(DungeonStart)로
        if (currentDungeon < 3)
        {
            SetPhase(GamePhase.DungeonStart);
        }
        else
        {
            SetPhase(GamePhase.GameEnd);
        }
    }

    private void ExecuteGameEnd()
    {
        //게임결과 보여주고 로비로 돌아갈 수 있는 창을 띄우면 될듯?
    }

    #endregion

}
