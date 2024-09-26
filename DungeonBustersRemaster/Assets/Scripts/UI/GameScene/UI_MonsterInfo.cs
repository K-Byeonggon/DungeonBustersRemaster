using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_MonsterInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_MonsterName;
    [SerializeField] TextMeshProUGUI Text_MonsterHP;
    [SerializeField] Transform Layout_Reward;


    private void SetMonsterInfo(int monsterDataId)
    {
        UpdateMonsterName(monsterDataId);
        UpdateMonsterHP(monsterDataId);
        UpdateReward(monsterDataId);
    }

    private void UpdateMonsterName(int monsterDataId)
    {
        Text_MonsterName.text = MonsterDataManager.Instance.LoadedMonsters[monsterDataId].Name;
    }

    private void UpdateMonsterHP(int monsterDataId)
    {
        int hp = MonsterDataManager.Instance.LoadedMonsters[monsterDataId].HP;
        Text_MonsterHP.text = $"HP: {hp}";
    }

    private void UpdateReward(int monsterDataId)
    {
        List<List<int>> rewardsData = MonsterDataManager.Instance.LoadedMonsters[monsterDataId].Reward;

        for (int i = 0; i < rewardsData.Count; i++)
        {
            GameObject reward = Layout_Reward.GetChild(i).gameObject;
            if (rewardsData[i].Count > 0)
            {
                Panel_Reward rewardPanel = reward.GetComponent<Panel_Reward>();
                rewardPanel.UpdateRewardInfo(rewardsData[i]);
                reward.SetActive(true);
            }
            else
            {
                reward.SetActive(false);
            }
        }
    }

    //GameLogicManager의 currentMonsterDataId가 변경될 때 hook으로 불림
    public static void UpdateMonsterInfo(int currentMonsterDataId)
    {
        UI_MonsterInfo monsterInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.MonsterInfoUI).GetComponent<UI_MonsterInfo>();
        monsterInfoUI.SetMonsterInfo(currentMonsterDataId);
    }
}
