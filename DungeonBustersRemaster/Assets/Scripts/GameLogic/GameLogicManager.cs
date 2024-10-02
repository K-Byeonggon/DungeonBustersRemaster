using Mirror;
using Mirror.Examples.CCU;
using System;
using System.Collections.Generic;
using System.Linq;
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
    StageEnd,
    DungeonEnd,
    GameEnd
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

    private HashSet<uint> playerResultChecked = new HashSet<uint>();

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
        currentStage = 0;
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
        currentDungeon++;
        //이번 던전의 몬스터 큐 뽑기(4마리의 몬스터)
        List<int> monsterIdList = MonsterDataManager.Instance.GetRandomMonsterDataIdsByDungeon(currentDungeon);
        currentDungeonMonsterIds = new Queue<int>(monsterIdList);
    }

    #endregion

    #region StageStart

    private void ExecuteStageStart()
    {
        //이번 스테이지에 등장할 몬스터가 결정된다.

        //다음 몬스터를 향해 달려가는 애니메이션이 재생된다.


        ServerExecuteStageStart();


        RpcExecuteStageStart();

        //SetPhase(GamePhase.CardSubmission);
        if (isServer)
        {
            RpcSetPhase(GamePhase.CardSubmission);

        }
    }
    [Server]
    private void ServerExecuteStageStart()
    {
        currentStage++;
        playerResultChecked.Clear();
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

    //PlayerGameData의 Command를 통해서 호출
    [Server]
    public void RegisterBattleResultChecked(uint netId)
    {
        playerResultChecked.Add(netId);

        if(playerResultChecked.Count == gamePlayerCount)
        {
            OnAllPlayersCheckedBattleResult();
        }
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
        playerResultChecked.Clear();

        RpcDebugAttackSuccessedList();
        //승리후 보석분배, 보너스보석 분배 / 패배후 보석회수가 이루어진다.

        //승리했을 경우
        if (isWin)
        {
            //보석분배
            //가장 적게 딜한 플레이어부터 Reward를 나눠가짐.
            //그러려면 공격성공한 플레이어들을 딜 순서로 정렬한 리스트가 있어야 할듯.
            //그거 Calculator에 있지 않음? 없어서 만듬.



            for(int i = 0; i < calculator.AttackSuccessedList.Count; i++)
            {
                //보상받을 플레이어가 없을 경우 예외처리
                if (i > calculator.AttackSuccessedList.Count - 1) break;

                PlayerCardInfo playerInfo = calculator.AttackSuccessedList[i];
                NetworkIdentity identity = NetworkClient.spawned[playerInfo.NetId];
                MyPlayerGameData playerGameData = identity.GetComponent<MyPlayerGameData>();

                List<int> reward = MonsterDataManager.Instance.LoadedMonsters[currentMonsterDataId].Reward[i];

                Debug.Log("reward: "+ string.Join(", ", reward));

                int[] arrayReward = reward.ToArray();

                playerGameData.CmdGetReward(arrayReward);
            }

        }
        //패배했을 경우
        else
        {
            RpcShowLoseGemsUI();

            //보석잃는 플레이어의 선택이 끝나면 이벤트 처리로 다음 Phase로 넘어감.
        }
    }

    [ClientRpc]
    private void RpcDebugAttackSuccessedList()
    {
        foreach(PlayerCardInfo player in calculator.AttackSuccessedList)
        {
            Debug.Log($"Player{player.NetId}(attackSuccessed) CardNum: {player.CardNumber}");
        }
    }

    #region 패배

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
                playerResultChecked.Add(NetworkClient.localPlayer.netId);
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
    public void RegisterLoseGemResultChecked(uint netId)
    {
        playerResultChecked.Add(netId);

        if (playerResultChecked.Count == gamePlayerCount)
        {
            OnAllMinAttackPlayerLoseGems();
        }
    }


    [Server]
    private void OnAllMinAttackPlayerLoseGems()
    {
        //UI 숨기기
        RpcHideWaitForOtherUI();

        RpcSetPhase(GamePhase.StageEnd);
    }

    [ClientRpc]
    private void RpcHideWaitForOtherUI()
    {
        UIManager.Instance.HideUIWithPooling(UIPrefab.LoseGemsUI);
        UIManager.Instance.HideUIWithPooling(UIPrefab.WaitForOtherUI);
    }

    #endregion

    [Server]
    private void OnAllPlayerGetBonusGems()
    {
        //승리해서 모든 플레이어가 보너스 보석 획득을 마쳤으면.

        RpcSetPhase(GamePhase.StageEnd);
    }


    [Command(requiresAuthority = false)]
    public void CmdSubBonusGems(GemColor color, int subAmount)
    {
        bonusGems[(int)color] -= subAmount;
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
