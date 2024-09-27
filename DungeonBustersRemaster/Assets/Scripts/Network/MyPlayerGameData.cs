using Mirror;
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

    [SyncVar(hook = nameof(OnIsAttackSuccessChanged))]
    private bool isAttackSuccess;

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
        List<int> tempList = new List<int>(usedCards);
        tempList.Add(card);
        tempList.Sort();

        usedCards.Clear();
        usedCards.AddRange(tempList);
    }

    [Server]
    public void SetGems(List<int> newGems)
    {
        gems.Clear();
        gems.AddRange(newGems);
    }

    [Server]
    public void AddGem(GemColor color, int amount)
    {
        if(amount < 0)
        {
            Debug.LogError($"Add amount < 0");
            return;
        }

        gems[(int)color] += amount;
    }

    [Server]
    public int LoseAllGemByColor(GemColor color)
    {
        int temp = gems[(int)color];
        gems[(int)color] = 0;
        return temp;
    }

    #region hook
    private void OnHandsChanged(SyncList<int>.Operation op, int oldItem, int newItem)
    {
        //UI변경
        //gameSceneUI.뭐시기저시기(netId);

    }

    private void OnGemsChanged(SyncList<int>.Operation op, int oldItem, int newItem)
    {
        //UI변경
        playerInfoUI.UpdatePlayerGemsInfo(netId);
    }

    private void OnUsedCardsChanged(SyncList<int>.Operation op, int oldItem, int newItem)
    {
        //UI변경
        playerInfoUI.UpdatePlayerUsedCardsInfo(netId);
    }

    private void OnSubmittedCardNumChanged(int oldNum, int newNum)
    {
        //Panel_SelectCard에서는 Local플레이어를 참조해서 이걸로 변경할 UI없음.
        //하지만 각 플레이어의 SubmittedCardNum은 모든 클라의 OpenCardUI에서 값을 알아야하므로 SyncVar이어야함.
    }

    private void OnIsAttackSuccessChanged(bool oldBool, bool newBool)
    {

    }
    #endregion
}
