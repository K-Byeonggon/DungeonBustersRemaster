using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//공격 성공한 카드의 정렬용.
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
    private Dictionary<uint, int> submittedCardNums = new Dictionary<uint, int>();
    private Dictionary<int, int> cardCount = new Dictionary<int, int>();    // 각 카드 번호의 등장 횟수를 기록
    
    private List<PlayerCardInfo> attackSuccessedList = new List<PlayerCardInfo>();

    public List<PlayerCardInfo> AttackSuccessedList => attackSuccessedList;

    public void SetSubmittedCardNums()
    {
        //cardCount초기화
        cardCount.Clear();

        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                submittedCardNums[kvp.Key] = playerGameData.SubmittedCardNum;

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

        LogSubmittedCardNums();

        SetAttackSuccess();
    }
    public void LogSubmittedCardNums()
    {
        foreach (var entry in submittedCardNums)
        {
            Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
        }
    }

    private void SetAttackSuccess()
    {
        // 중복된 카드 번호를 가진 플레이어의 공격 성공 여부를 설정
        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                int submittedCardNum = playerGameData.SubmittedCardNum;
                bool attackSuccessed = cardCount[submittedCardNum] == 1;
                playerGameData.IsAttackSuccess = attackSuccessed;

                //공격 성공리스트 등록
                if (attackSuccessed)
                {
                    attackSuccessedList.Add(new PlayerCardInfo(playerGameData.netId, submittedCardNum));
                }
            }
        }

        //공격 성공리스트 정렬
        attackSuccessedList.Sort((x, y) => x.CardNumber.CompareTo(y.CardNumber));
    }



    [Server]
    public int GetSumOfAttack()
    {
        int nonDuplicateSum = cardCount
            .Where(kvp => kvp.Value == 1)  // 등장 횟수가 1인 카드 번호만 필터링
            .Sum(kvp => kvp.Key);          // 그들의 Key (카드 번호)를 합산

        return nonDuplicateSum;
    }
}
