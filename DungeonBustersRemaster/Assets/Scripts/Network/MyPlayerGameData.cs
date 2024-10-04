using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyPlayerGameData : NetworkBehaviour
{

    //hands : 아직 사용하지 않은 플레이어가 가진 카드들
    private readonly SyncList<int> hands = new SyncList<int>();

    //gems : 플레이어가 가진 보석들 
    private readonly SyncList<int> gems = new SyncList<int>();

    //usedCards : 플레이어가 사용한 카드들
    private readonly SyncList<int> usedCards = new SyncList<int>();

    private UI_PlayerInfo playerInfoUI;

    [SyncVar(hook = nameof(OnSubmittedCardNumChanged))]
    private int submittedCardNum;

    [SyncVar(hook = nameof(OnIsCardSubmittedChanged))]
    private bool isCardSubmitted;

    [SyncVar(hook = nameof(OnIsAttackSuccessChanged))]
    private bool isAttackSuccess;

    [SyncVar(hook = nameof(OnIsMinAttackPlayerChanged))]
    private bool isMinAttackPlayer;

    [SyncVar(hook = nameof(OnCurrentStageRankChanged))]
    private int currentStageRank;

    public List<int> Hands => hands.ToList();
    public List<int> Gems => gems.ToList();
    public List<int> UsedCards => usedCards.ToList();

    public int SubmittedCardNum
    {
        get => submittedCardNum;
        set { submittedCardNum = value; }
    }

    public bool IsAttackSuccess
    {
        get => isAttackSuccess;
        set { isAttackSuccess = value; }
    }

    public bool IsCardSubmitted
    {
        get => isCardSubmitted;
        set { isCardSubmitted = value; }
    }

    public bool IsMinAttackPlayer
    {
        get => isMinAttackPlayer;
        set { isMinAttackPlayer = value; }
    }

    public int CurrentStageRank
    {
        get => currentStageRank;
        set { currentStageRank = value; }
    }

    public override void OnStartClient()
    {
        //SyncList는 SyncVar와 hook함수를 사용하지 않는다.
        hands.OnChange += OnHandsChanged;
        gems.OnChange += OnGemsChanged;
        usedCards.OnChange += OnUsedCardsChanged;

        //UI에 플레이어 Panel 생성
        playerInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.PlayerInfoUI).GetComponent<UI_PlayerInfo>();
        playerInfoUI.SetPlayerInfo(netId);
    }

    public override void OnStopClient()
    {
        hands.OnChange -= OnHandsChanged;
        gems.OnChange -= OnGemsChanged;
        usedCards.OnChange -= OnUsedCardsChanged;

        playerInfoUI.RemovePlayerInfo(netId);
    }

    //SyncList의 변경은 Server에서만 이루어진다.
    //Initialize는 MyNetWorkRoomManager에서 GamePlayer가 생성될 때 불림
    [Server]
    public void InitializeHands()
    {
        List<int> list = new List<int>() { 1,2,3,4,5,6,7 };
        if(MyNetworkRoomManager.Instance.minPlayers < 4)
        {
            list.Remove(1);
        }
        else
        {
            list.Remove(7);
        }
        hands.Clear();
        hands.AddRange(list);
    }

    [Server]
    public void InitializeUsedCards()
    {
        //Clear를 사용하니까 바로 UI에 반영해야되는 UsedCards는 동기화 지연 문제가 발생한다.
        usedCards.Clear();

    }

    [Server]
    public void InitializeGems()
    {
        List<int> list = new List<int>() { 1, 1, 1 };
        gems.Clear();
        gems.AddRange(list);
    }


    //OnStartServer에서 MyPlayer가 호출.
    [Server]
    public void InitialCardSettings()
    {
        InitializeHands();
        InitializeUsedCards();
    }

    [Server]
    public void RemoveHand(int card)
    {
        hands.Remove(card);
    }

    [Server]
    public void AddUsedCard(int card)
    {
        usedCards.Add(card);
    }


    //DungeonStart에서 초기화하도록
    [Command(requiresAuthority = false)]
    public void CmdInitializeCardSettings()
    {
        InitialCardSettings();
    }

    //서버로 쏴주는 방법 말고 서버로 변경 요청하기
    [Command(requiresAuthority = false)]
    public void CmdSetSubmittedCardNum(int newCardNum)
    {
        submittedCardNum = newCardNum;
    }


    //서버로 Hands와 UsedCards 변경 요청
    [Command(requiresAuthority = false)]
    public void CmdUpdatePlayerCardInfo()
    {
        AddUsedCard(submittedCardNum);
        RemoveHand(submittedCardNum);
    }


    //서버로 Reward 받아서 Gems 변경 요청(array는 되는걸로 안다)
    [Command(requiresAuthority = false)]
    public void CmdGetReward(int[] arrayReward)
    {
        if(arrayReward.Length == 0 || arrayReward == null)
        {
            Debug.LogWarning("reward is empty");
            return;
        }

        for(int i = 0;  i < arrayReward.Length; i++)
        {
            if (arrayReward[i] > 0)
            {
                gems[i] += arrayReward[i];
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdLoseAllGemsByColor(GemColor color)
    {
        int loseGemCount = gems[(int)color];

        gems[(int)color] = 0;

        GameLogicManager.Instance.AddBonusGemsToLogicManager(color, loseGemCount);
    }

    //GameManager의 calculator를 통해서 플레이어 보상 순서 결정.
    [Command(requiresAuthority = false)]
    public void CmdUpdateCurrentStageRank(int newRank)
    {
        currentStageRank = newRank;
    }

    [Command(requiresAuthority = false)]
    public void CmdAddGemsByColor(GemColor color, int count)
    {
        if (count < 0) return;

        gems[(int)color] += count;

        //LogicManager에 얻은만큼 빼는 것은 UI변경이 바로되어야해서 따로작성
    }

    #region hook
    private void OnHandsChanged(SyncList<int>.Operation op, int index, int newItem)
    {
        string message = $"Player{netId} OnHandsChanged: ";
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                message += $"Added {newItem} to Hands at index {index}";
                break;
            case SyncList<int>.Operation.OP_INSERT:
                message += $"Inserted {newItem} to Hands at index {index}";
                break;
            case SyncList<int>.Operation.OP_SET:
                message += $"Set Hands at index {index} to {newItem}";
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                message += $"Removed {newItem} from Hands at index {index}";
                break;
            case SyncList<int>.Operation.OP_CLEAR:
                message += "Cleared all Hands";
                break;
        }
        Debug.Log(message);
        //UI변경..을 SelectCard에서 OnEnable될때 알아서 한다.

    }

    private void OnGemsChanged(SyncList<int>.Operation op, int index, int newItem)
    {
        string message = $"Player{netId} OnGemsChanged: ";
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                message += $"Added {newItem} to Gems at index {index}";
                break;
            case SyncList<int>.Operation.OP_INSERT:
                message += $"Inserted {newItem} to Gems at index {index}";
                break;
            case SyncList<int>.Operation.OP_SET:
                message += $"Set Gems at index {index} to {newItem}";
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                message += $"Removed {newItem} from Gems at index {index}";
                break;
            case SyncList<int>.Operation.OP_CLEAR:
                message += "Cleared all Gems";
                break;
        }
        Debug.Log(message);


        //UI변경
        playerInfoUI.UpdatePlayerGemsInfo(netId);
    }

    private void OnUsedCardsChanged(SyncList<int>.Operation op, int index, int newItem)
    {
        string message = $"Player{netId} OnUsedCardChanged: ";
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                message += $"Added {newItem} to UsedCards at index {index}";
                playerInfoUI.UpdatePlayerUsedCardsInfo(netId);
                break;
            case SyncList<int>.Operation.OP_INSERT:
                message += $"Inserted {newItem} to UsedCards at index {index}";
                playerInfoUI.UpdatePlayerUsedCardsInfo(netId);
                break;
            case SyncList<int>.Operation.OP_SET:
                message += $"Set UsedCards at index {index} to {newItem}";
                playerInfoUI.UpdatePlayerUsedCardsInfo(netId);
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                message += $"Removed {newItem} from UsedCards at index {index}";
                playerInfoUI.UpdatePlayerUsedCardsInfo(netId);
                break;
                
            case SyncList<int>.Operation.OP_CLEAR:
                message += "Cleared all UsedCards";
                //.Clear는 동기화가 느려서 강제 UI업데이트를 부른다.
                playerInfoUI.ForceUpdatePlayerUsedCardsInfo(new List<int>(), netId);
                break;
        }
        Debug.Log(message);

        //UI변경
    }


    private void OnSubmittedCardNumChanged(int oldNum, int newNum)
    {
        Debug.Log($"Player{netId} SubmittedCardNumChanged: {oldNum} -> {newNum}");

        //UI변경
        UI_CardPanel cardPanelUI = UIManager.Instance.GetActiveUI(UIPrefab.CardPanelUI).GetComponent<UI_CardPanel>();
        cardPanelUI.UpdateSelectedCard();
    }

    private void OnIsAttackSuccessChanged(bool oldBool, bool newBool)
    {

    }

    private void OnIsCardSubmittedChanged(bool oldBool, bool newBool)
    {

    }

    private void OnCurrentStageRankChanged(int oldRank, int newRank)
    {
        Debug.Log($"Player{netId} CurrentStageRankChanged: {oldRank} -> {newRank}");

    }

    private void OnIsMinAttackPlayerChanged(bool oldBool, bool newBool)
    {

    }
    #endregion
}
