using Mirror;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PlayerCardInfo
{
    public uint NetId { get; set; }
    public int CardNumber { get; set; }

    public PlayerCardInfo(uint netId, int cardNumber)
    {
        NetId = netId;
        CardNumber = cardNumber;
    }
}

public class ResultCalculator
{
    // 계산기에 필요한 것 1, 2, 3
    private List<PlayerCardInfo> submittedCardNums = new List<PlayerCardInfo>();
    private Dictionary<int, int> cardCount = new Dictionary<int, int>(); // 각 카드 번호의 등장 횟수를 기록
    private Queue<uint> rewardOrder = new Queue<uint>();

    // 1. 공격에 성공한 플레이어를 적은 카드 순서대로 정렬한 리스트.
    private List<PlayerCardInfo> attackSuccessedList = new List<PlayerCardInfo>();

    public List<PlayerCardInfo> AttackSuccessedList => attackSuccessedList;

    public Queue<uint> RewardOrder => rewardOrder;

    public void SetSubmittedCardNums()
    {
        // cardCount 초기화
        cardCount.Clear();
        submittedCardNums.Clear();  // 새롭게 제출된 카드 리스트 초기화

        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                var playerCardInfo = new PlayerCardInfo(kvp.Key, playerGameData.SubmittedCardNum);
                submittedCardNums.Add(playerCardInfo);

                // 제출된 카드 번호 등장 횟수 증가
                if (cardCount.ContainsKey(playerGameData.SubmittedCardNum))
                {
                    cardCount[playerGameData.SubmittedCardNum]++;
                }
                else
                {
                    cardCount[playerGameData.SubmittedCardNum] = 1;
                }
            }
        }


        // 공격 리스트 정렬
        submittedCardNums.Sort((x, y) => x.CardNumber.CompareTo(y.CardNumber));

        LogSubmittedCardNums();

        SetAttackSuccess();

        SetRewardOrder();

        SetMinAttackPlayer();
    }

    public void LogSubmittedCardNums()
    {
        foreach (var entry in submittedCardNums)
        {
            Debug.Log($"NetId: {entry.NetId}, CardNumber: {entry.CardNumber}");
        }
    }

    private void SetAttackSuccess()
    {
        // 중복된 카드 번호를 가진 플레이어의 공격 성공 여부를 설정
        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                //2. 플레이어의 공격 성공여부 갱신
                int submittedCardNum = playerGameData.SubmittedCardNum;
                bool attackSuccessed = cardCount[submittedCardNum] == 1;
                playerGameData.IsAttackSuccess = attackSuccessed;

                // 공격 성공 리스트 등록
                if (attackSuccessed)
                {
                    attackSuccessedList.Add(new PlayerCardInfo(playerGameData.netId, submittedCardNum));
                }
            }
        }

        // 공격 성공 리스트 정렬
        attackSuccessedList.Sort((x, y) => x.CardNumber.CompareTo(y.CardNumber));
    }

    //정렬방식: 공격 성공리스트가 순서대로 넣어짐. 공격 실패한 것들은 그 뒤로 넣어짐.
    //공격 성공한 플레이어의 순위: 0, 1, 2, ...  실패 플레이어 순위: -1
    private void SetRewardOrder()
    {
        //attackSuccessedList로 플레이어 순위 변경(0, 1, 2, ...)(-1)
        foreach(var kvp in NetworkClient.spawned)
        {
            if(kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                //FindIndex는 원하는 조건을 찾지 못하면 -1을 반환한다.
                int currentStageRank = attackSuccessedList.FindIndex(x => x.NetId == playerGameData.netId);
                playerGameData.CmdUpdateCurrentStageRank(currentStageRank);
            }
        }
    }

    //3. 플레이어의 isMinAttackPlayer 갱신
    private void SetMinAttackPlayer()
    {
        // 가장 작은 카드 번호
        int minValue = submittedCardNums.Min(cardInfo => cardInfo.CardNumber);

        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                int submittedCardNum = playerGameData.SubmittedCardNum;
                if(submittedCardNum == minValue)
                {
                    playerGameData.IsMinAttackPlayer = true;
                }
                else
                {
                    playerGameData.IsMinAttackPlayer = false;
                }
            }
        }
    }

    [Server]
    public int GetSumOfAttack()
    {
        int nonDuplicateSum = cardCount
            .Where(kvp => kvp.Value == 1)  // 등장 횟수가 1인 카드 번호만 필터링
            .Sum(kvp => kvp.Key);          // 그들의 Key (카드 번호)를 합산

        return nonDuplicateSum;
    }

    [Server]
    public List<uint> GetNetIdsWithMinSubmittedCard()
    {
        // List가 비어있으면 빈 리스트 반환
        if (submittedCardNums.Count == 0)
        {
            return new List<uint>();
        }

        // 가장 작은 카드 번호 찾기
        int minValue = submittedCardNums.Min(cardInfo => cardInfo.CardNumber);

        // 가장 작은 카드 번호를 가진 플레이어들의 NetId 리스트 추출
        List<uint> minValueKeys = submittedCardNums
                                  .Where(cardInfo => cardInfo.CardNumber == minValue)
                                  .Select(cardInfo => cardInfo.NetId)
                                  .ToList();

        return minValueKeys;
    }


}
