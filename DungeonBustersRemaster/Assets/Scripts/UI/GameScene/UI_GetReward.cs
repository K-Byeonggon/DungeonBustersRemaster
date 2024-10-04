using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetReward : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Message;
    [SerializeField] Transform Layout_Reward;
    [SerializeField] Button Btn_Confirm;

    private List<int> currentReward; 

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void OnClick_Confirm()
    {
        //UI숨기기
        UIManager.Instance.HideUIWithPooling(UIPrefab.GetRewardUI);

        //reward로 보상 얻는 Cmd보내기
        if(currentReward != null)
        {
            MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
            playerGameData.CmdGetReward(currentReward.ToArray());

            Debug.Log("Reward received: " + string.Join(", ", currentReward));
        }
        else
        {
            Debug.LogError("UI_GetReward: currentReward is null");
        }

        //GameLogicManger에 신호보내기
        //GameLogicManager.Instance.CmdRegisterGetRewardChecked(NetworkClient.localPlayer.netId);
        GameLogicManager.Instance.CmdCheckConfirm(NetworkClient.localPlayer.netId, ConfirmPhase.GetRewardResult);
    }

    private void SetLayoutReward(List<int> reward, int currentRank)
    {
        foreach(Transform gem in Layout_Reward)
        {
            Destroy(gem.gameObject);
        }

        Text_Message.text = $"{currentRank + 1}번째로 적은 피해를 주었습니다. \n{currentRank + 1}번째 보상을 획득합니다.";

        currentReward = reward;

        GameObject prefabGem = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Gem);
        for(int index = 0;  index < reward.Count; index++)
        {
            for(int i = 0; i < reward[index];  i++)
            {
                GameObject gObj = Instantiate(prefabGem, Layout_Reward);
                Content_Gem gem = gObj.GetComponent<Content_Gem>();
                gem.SetGemImg((GemColor)index);
            }
        }
    }


    public static void Show(int currentMonsterDataId)
    {        
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        int currentRank = playerGameData.CurrentStageRank;

        Debug.Log($"<color=red> currentRank: {currentRank} </color>");

        //공격에 실패하거나, 순위가 최대 보상개수(3개) 보다 큰 플레이어들
        List<List<int>> rewards = MonsterDataManager.Instance.LoadedMonsters[currentMonsterDataId].Reward;

        Debug.Log($"<color=red> rewards.Count: {rewards.Count} </color>");

        //순위가 실제 보상개수(1~3개) 이하인 플레이어
        List<int> reward = rewards[currentRank];
        if (currentRank >= 0 && reward != null && reward.Count > 0)
        {
            UIManager.Instance.ShowUI(UIPrefab.GetRewardUI);
            UI_GetReward getRewardUI = UIManager.Instance.GetActiveUI(UIPrefab.GetRewardUI).GetComponent<UI_GetReward>();

            getRewardUI.SetLayoutReward(reward, currentRank);
        }
        else
        {
            UI_WaitForOther.Show("다른 플레이어가 보상 받는 것을 기다리고 있습니다.");
            //LogicManager에 확인 신호보내기
            GameLogicManager.Instance.CmdCheckConfirm(NetworkClient.localPlayer.netId, ConfirmPhase.GetRewardResult);
        }
    }

}
