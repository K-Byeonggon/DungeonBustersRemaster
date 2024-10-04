using Mirror;
using Mirror.Examples.CCU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum GamePhase
{
    BeforeGameStart,
    GameStart,
    DungeonStart,
    StageStart,
    CardSubmission,
    ResultCalculation,
    RewardDistribution,
    BonusDistribution,
    StageEnd,
    DungeonEnd,
    GameEnd
}

public enum ConfirmPhase
{
    StageBattleResult,
    GetRewardResult,
    LoseGemsResult,
    GetBonusResult
}

public class GameLogicManager : NetworkBehaviour
{
    //NetworkBehaviour에 Singleton패턴을 적용하면 NetworkManager에 의한 관리와 충돌할 수 있다.
    public static GameLogicManager Instance;

    [SerializeField] MonsterSpawner monsterSpawner;

    private ResultCalculator calculator;


    private int gamePlayerCount;


    private GamePhase currentPhase = GamePhase.BeforeGameStart;


    private Dictionary<GamePhase, Action> phaseActions;

    [SyncVar(hook = nameof(OnCurrentDungeonChanged))]
    private int currentDungeon;

    private Queue<int> currentDungeonMonsterIds = new Queue<int>();

    [SyncVar(hook = nameof(OnCurrentStageChanged))]
    private int currentStage;

    private int currentMonsterDataId;

    private readonly SyncList<int> bonusGems = new SyncList<int>();

    [SyncVar(hook = nameof(OnSumittedPlayerCountChanged))]
    private int submittedPlayerCount;

    [SyncVar(hook = nameof(OnIsWinChanged))]
    private bool isWin;

    private int currentBonusPlayerIndex;

    //플레이어가 확인했는지 여부를 저장함. 어떤 단계를 확인했는지 저장해서 다른 동작을 함.
    private Dictionary<uint, ConfirmPhase> playerConfirmations = new Dictionary<uint, ConfirmPhase>();

    public List<int> BonusGems => bonusGems.ToList<int>();


    #region Property

    public int CurrentMonsterDataId
    {
        get => currentMonsterDataId;
        set
        {
            int oldDataId = currentMonsterDataId;
            currentMonsterDataId = value;
            OnCurrentMonsterDataIdChanged(oldDataId, currentMonsterDataId);
        }
    }

    public GamePhase CurrentPhase
    {
        get => currentPhase;
        set
        {
            GamePhase oldPhase = currentPhase;
            currentPhase = value;
            OnPhaseChanged(oldPhase, currentPhase);
        }
    }

    public bool IsWin => isWin;

    #endregion

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

        Debug.Log("<color=red> OnBonusGemsChanged </color>");
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                UI_BonusGems.UpdateBonusGems(BonusGems);
                if(UIManager.Instance.GetActiveUI(UIPrefab.LoseGemsUI) != null)
                {
                    UI_LoseGems.UpdateBonusGems(BonusGems);
                }
                break;
            case SyncList<int>.Operation.OP_SET:
                UI_BonusGems.UpdateBonusGems(BonusGems); 
                if (UIManager.Instance.GetActiveUI(UIPrefab.LoseGemsUI) != null)
                {
                    UI_LoseGems.UpdateBonusGems(BonusGems);
                }
                if (UIManager.Instance.GetActiveUI(UIPrefab.GetBonusUI) != null)
                {

                }
                break;
        }
    }

    private void OnSumittedPlayerCountChanged(int oldCount, int newCount)
    {
        Debug.Log($"SubmittedPlayerCountChanged: {oldCount} -> {newCount}");
        if (isServer)
        {
            ServerCheckSubmittedPlayerCount();
        }
    }

    private void OnIsWinChanged(bool oldBool, bool newBool)
    {
        Debug.Log($"isWinChanged: {oldBool} -> {newBool}");

    }
    #endregion

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
            { GamePhase.BonusDistribution, ExecuteBonusDistribution },
            { GamePhase.StageEnd, ExecuteStageEnd },
            { GamePhase.DungeonEnd, ExecuteDungeonEnd },
            { GamePhase.GameEnd, ExecuteGameEnd }
        };
    }


    [Server]
    private void StartGame(int gamePlayerCount)
    {
        this.gamePlayerCount = gamePlayerCount;
        Debug.Log(this.gamePlayerCount + "명의 GamePlayer");
        
        if (isServer)
        {
            RpcSetPhase(GamePhase.GameStart);
        }

    }


    [ClientRpc]
    private void RpcSetPhase(GamePhase newPhase)
    {
        //페이즈 변경은 모든 클라에서
        CurrentPhase = newPhase;

        //페이즈의 수행은 서버에서만
        if (isServer)
        {
            ServerExecuteCurrentPhase();
        }
    }

    [Server]
    private void ServerExecuteCurrentPhase()
    {
        if (phaseActions.TryGetValue(currentPhase, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogError($"No action defind for rpcphase {currentPhase}");
        }
    }

    //모든 플레이어의 확인을 받는 시스템
    [Server]
    private void ServerCheckConfirm(uint netId, ConfirmPhase phase)
    {
        if(playerConfirmations.ContainsKey(netId))
        {
            playerConfirmations[netId] = phase;
        }
        else
        {
            playerConfirmations.Add(netId, phase);
        }

        CheckIfAllPlayersConfirmed(phase);
    }

    [Server]
    private void CheckIfAllPlayersConfirmed(ConfirmPhase phase)
    {
        if(playerConfirmations.Count != gamePlayerCount)
        {
            return;
        }

        if(playerConfirmations.Values.All(p => p == phase))
        {
            Debug.Log($"All players confirmed for {phase}");

            RpcHideWaitForOthersUI();

            switch (phase)
            {
                case ConfirmPhase.StageBattleResult:
                    OnAllPlayersCheckedBattleResult();
                    break;
                case ConfirmPhase.GetRewardResult:
                    OnAllPlayerGetReward();
                    break;
                case ConfirmPhase.LoseGemsResult:
                    OnAllMinAttackPlayerLoseGems();
                    break;
                case ConfirmPhase.GetBonusResult:
                    break;
                default:
                    Debug.LogWarning("Unhandled phase");
                    break;
            }

        }
    }

    [ClientRpc]
    private void RpcHideWaitForOthersUI()
    {
        UIManager.Instance.HideUIWithPooling(UIPrefab.WaitForOtherUI);
    }

    [Command(requiresAuthority = false)]
    public void CmdCheckConfirm(uint netId, ConfirmPhase phase)
    {
        ServerCheckConfirm(netId, phase);
    }


    #region Phase Action

    #region GameStart
    [Server]
    private void ExecuteGameStart()
    {
        Debug.Log("Game Start Phase");

        ServerInitializeGame();

        RpcSetPhase(GamePhase.DungeonStart);

    }


    [Server]
    private void ServerInitializeGame()
    {
        //서버에서만 수행되는 초기화 과정
        currentDungeon = 0;
        currentDungeonMonsterIds.Clear();
        bonusGems.Clear();
        bonusGems.AddRange(new List<int>() { 0, 0, 0 });
    }

    #endregion

    #region DungeonStart
    [Server]
    private void ExecuteDungeonStart()
    {
        ServerExecuteDungeonStart();

        //SetPhase(GamePhase.StageStart);
        if (isServer)
        {
            RpcSetPhase(GamePhase.StageStart);

        }
    }

    [Server]
    private void ServerExecuteDungeonStart()
    {
        currentStage = 0;
        currentDungeon++;
        RpcRequestInitializeCardSettings();

        //이번 던전의 몬스터 큐 뽑기(4마리의 몬스터)
        List<int> monsterIdList = MonsterDataManager.Instance.GetRandomMonsterDataIdsByDungeon(currentDungeon);
        currentDungeonMonsterIds = new Queue<int>(monsterIdList);
    }

    [ClientRpc]
    private void RpcRequestInitializeCardSettings()
    {
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        playerGameData.CmdInitializeCardSettings();
    }

    #endregion

    #region StageStart

    private void ExecuteStageStart()
    {
        //이번 스테이지에 등장할 몬스터가 결정된다.

        //다음 몬스터를 향해 달려가는 애니메이션이 재생된다.

        ServerExecuteStageStart();

        RpcExecuteStageStart();

        if (isServer)
        {
            RpcSetPhase(GamePhase.CardSubmission);

        }
    }
    [Server]
    private void ServerExecuteStageStart()
    {
        currentStage++;
        submittedPlayerCount = 0;
        RpcInitializePlayerSubmittedCard();

        //Deque는 서버에서, 적용은 Rpc로
        currentMonsterDataId = currentDungeonMonsterIds.Dequeue();
        RpcSetCurrentMonsterDataId(currentMonsterDataId);
    }
    [ClientRpc]
    private void RpcSetCurrentMonsterDataId(int serverMonsterDataId)
    {
        //몬스터 ID변경 (UI 변경)
        CurrentMonsterDataId = serverMonsterDataId;

        //몬스터 스폰
        monsterSpawner.SpawnMonster(serverMonsterDataId);
    }

    [ClientRpc]
    private void RpcInitializePlayerSubmittedCard()
    {
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        playerGameData.CmdSetSubmittedCardNum(0);

        UI_CardPanel cardPanelUI = UIManager.Instance.GetActiveUI(UIPrefab.CardPanelUI).GetComponent<UI_CardPanel>();
        cardPanelUI.Initialize();
    }


    [ClientRpc]
    private void RpcExecuteStageStart()
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
        InitializeCardPanel();
        //모든 플레이어가 카드를 제출할 때까지 대기한다.
    }

    [ClientRpc]
    private void InitializeCardPanel()
    {
        UI_CardPanel cardPanelUI = UIManager.Instance.GetActiveUI(UIPrefab.CardPanelUI).GetComponent<UI_CardPanel>();
        cardPanelUI.UpdateSelectedCard();
    }

    [Command(requiresAuthority = false)]
    public void CmdAddSubmittedPlayerCount()
    {
        if(currentPhase == GamePhase.CardSubmission)
        {
            submittedPlayerCount++;
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdSubSubmittedPlayerCount()
    {
        if(currentPhase == GamePhase.CardSubmission && submittedPlayerCount > 0)
        {
            submittedPlayerCount--;
        }
    }

    [Server]
    private void ServerCheckSubmittedPlayerCount()
    {
        if (submittedPlayerCount == gamePlayerCount)
        {
            OnAllPlayerSubmittedCard();
        }
    }

    [Server]
    private void OnAllPlayerSubmittedCard()
    {
        //플레이어의 UsedCard, Hands변경
        RpcUpdatePlayersCardInfo();

        //다음 페이즈로
        RpcSetPhase(GamePhase.ResultCalculation);
    }

    [ClientRpc]
    private void RpcUpdatePlayersCardInfo()
    {
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        playerGameData.CmdUpdatePlayerCardInfo();
    }

    #endregion


    #region ResultCalculation
    private void ExecuteResultCalculation()
    {
        //계산기 초기화
        calculator = new ResultCalculator();

        //플레이어가 제출한 카드의 숫자로 현재 스테이지의 몬스터를 쓰러뜨릴수 있는지 계산한다.
        //플레이어의 공격 성공여부를 갱신해준다.
        calculator.SetSubmittedCardNums();
        int sumOfAttack = calculator.GetSumOfAttack();
        int monsterHP = MonsterDataManager.Instance.LoadedMonsters[currentMonsterDataId].HP;

        RpcDebugResult(sumOfAttack, monsterHP);
        
        if (sumOfAttack >= monsterHP)
        {
            isWin = true;
        }
        else
        {
            isWin = false;
        }

        //결과 확인 UI 띄워주기
        //isWin갱신을 기다릴 필요없이 Server에서 결과를 쏴주자.
        RpcShowResultUI(isWin);
    }

    [ClientRpc]
    private void RpcShowResultUI(bool isWin)
    {
        UI_WinLose.Show(isWin);
    }


    [Server]
    private void OnAllPlayersCheckedBattleResult()
    {
        //UI상으로 모든 플레이어가 결과 확인 버튼을 눌렀으면.

        RpcSetPhase(GamePhase.RewardDistribution);
    }


    [ClientRpc]
    private void RpcDebugResult(int sumOfAttack, int monsterHP)
    {

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

    #endregion


    #region RewardDistribution
    private void ExecuteRewardDistribution()
    {
        ServerDebugAttackSuccessedList();
        
        //승리후 보석분배, 보너스보석 분배 / 패배후 보석회수가 이루어진다.

        //승리했을 경우
        if (isWin)
        {
            //UI띄우기
            RpcShowGetRewardUI(currentMonsterDataId);

        }
        //패배했을 경우
        else
        {
            RpcShowLoseGemsUI();

            //보석잃는 플레이어의 선택이 끝나면 이벤트 처리로 다음 Phase로 넘어감.
        }
    }

    [Server]
    private void ServerDebugAttackSuccessedList()
    {
        foreach(PlayerCardInfo player in calculator.AttackSuccessedList)
        {
            Debug.Log($"Player{player.NetId}(attackSuccessed) CardNum: {player.CardNumber}");
        }
    }

    #region 패배(RewardDistribution)

    [ClientRpc]
    private void RpcShowLoseGemsUI()
    {
        if (NetworkClient.localPlayer != null && NetworkClient.localPlayer.TryGetComponent(out MyPlayerGameData playerGameData))
        {
            // Local의 gameData의 isMinAttackPlayer에 따라 다른 UI를 띄워준다.
            if (playerGameData.IsMinAttackPlayer)
            {
                UIManager.Instance.ShowUI(UIPrefab.LoseGemsUI);
            }
            else
            {
                CmdCheckConfirm(NetworkClient.localPlayer.netId, ConfirmPhase.LoseGemsResult);
                UI_WaitForOther.Show("열심히 공격하지 않은 플레이어가 벌을 받고 있습니다..");
            }
        }
        else
        {
            Debug.LogWarning("LocalPlayer or MyPlayerGameData not found.");
        }

    }

    //MyPlayerGameData에서 Cmd로 불러짐.
    [Server]
    public void AddBonusGemsToLogicManager(GemColor color, int gemCount)
    {
        bonusGems[(int)color] += gemCount;
    }


    [Server]
    private void OnAllMinAttackPlayerLoseGems()
    {
        RpcSetPhase(GamePhase.StageEnd);
    }


    #endregion

    #region 승리(RewardDistribution)

    [ClientRpc]
    private void RpcShowGetRewardUI(int monsterDataId)
    {
        UI_GetReward.Show(monsterDataId);
    }


    [Server]
    private void OnAllPlayerGetReward()
    {
        RpcSetPhase(GamePhase.BonusDistribution);
    }


    #endregion

    #endregion

    #region Bonus Distribution

    private void ExecuteBonusDistribution()
    {
        //보너스 없으면 다음 Phase로
        if(bonusGems.All(gem => gem == 0))
        {
            RpcSetPhase(GamePhase.StageEnd);
            return;
        }

        //보너스 클릭해서 가져갈 수 있는 UI 띄워주기
        currentBonusPlayerIndex = 0;
        uint currentGetBonusPlayerNetId = calculator.AttackSuccessedList[currentBonusPlayerIndex].NetId;
        int[] arrayBonus = BonusGems.ToArray();


        RpcShowGetBonusUI(arrayBonus, currentGetBonusPlayerNetId);

        //보너스 전부 떨어지면 OnAllPlayerGetBonus 실행.

    }

    [ClientRpc]
    private void RpcShowGetBonusUI(int[] arrayBonus, uint currentGetBonusPlayerNetId)
    {
        if(NetworkClient.localPlayer.netId == currentGetBonusPlayerNetId)
        {
            UIManager.Instance.HideUIWithPooling(UIPrefab.WaitForOtherUI);

            UI_GetBonus.Show(arrayBonus);
            
        }
        else
        {
            UI_WaitForOther.Show("다른 플레이어의 보너스 선택을 기다리는 중입니다..");
        }
    }

    private IEnumerator TestCoroutine()
    {
        yield return new WaitForSeconds(2f);
    }

    [Command(requiresAuthority = false)]
    public void CmdOnClickPanelBonusGem(GemColor color, int subAmount)
    {
        List<int> tempBonusGems = new List<int>(BonusGems);

        //이건 Sync라서 딜레이가 있음.
        bonusGems[(int)color] -= subAmount;

        //UI표시에는 tempBonusGems를 사용할 것임.
        tempBonusGems[(int)color] -= subAmount;
        int[] arrayBonus = tempBonusGems.ToArray();

        foreach (int gem in bonusGems)
        {
            if (gem != 0)
            {
                //다음 사람 보너스 선택하는 UI띄워주기
                currentBonusPlayerIndex = (currentBonusPlayerIndex + 1) % calculator.AttackSuccessedList.Count;
                uint currentGetBonusPlayerNetId = calculator.AttackSuccessedList[currentBonusPlayerIndex].NetId;

                RpcShowGetBonusUI(arrayBonus, currentGetBonusPlayerNetId);
                return;
            }
        }

        //보너스가 전부 소진되면
        OnAllPlayerGetBonus();
    }

    [Server]
    private void OnAllPlayerGetBonus()
    {
        RpcHideWaitForOthersUI();

        RpcSetPhase(GamePhase.StageEnd);
    }


    #endregion


    private void ExecuteStageEnd()
    {
        //아직 몬스터 남았으면 StageStart로
        if (currentDungeonMonsterIds.Count > 0)
        {
            RpcSetPhase(GamePhase.StageStart);
        }
        else
        {
            RpcSetPhase(GamePhase.DungeonEnd);
        }
    }

    private void ExecuteDungeonEnd()
    {
        //던전 3개 다돌았으면 SetPhase(GameEnd) 아니면 던전 번호 올려서 SetPhase(DungeonStart)로
        if (currentDungeon < 3)
        {
            RpcSetPhase(GamePhase.DungeonStart);
        }
        else
        {
            RpcSetPhase(GamePhase.GameEnd);
        }
    }

    private void ExecuteGameEnd()
    {
        //게임결과 보여주고 로비로 돌아갈 수 있는 창을 띄우면 될듯?
    }

    #endregion

}
