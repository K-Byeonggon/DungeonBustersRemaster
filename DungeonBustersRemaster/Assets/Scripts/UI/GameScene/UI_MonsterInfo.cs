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
        string name = string.Empty;
        if(monsterDataId != -1)
        {
            name = MonsterDataManager.Instance.LoadedMonsters[monsterDataId].Name;
        }
        else
        {
            name = "???";
        }
        Text_MonsterName.text = name;
    }

    private void UpdateMonsterHP(int monsterDataId)
    {
        string hp = string.Empty;
        if(monsterDataId != -1)
        {
            hp = MonsterDataManager.Instance.LoadedMonsters[monsterDataId].HP.ToString();
        }
        else
        {
            hp = "??";
        }

        Text_MonsterHP.text = $"HP: {hp}";
    }

    private void UpdateReward(int monsterDataId)
    {
        if (monsterDataId < 0)
        {
            foreach(Transform reward in Layout_Reward)
            {
                reward.gameObject.SetActive(false);
            }
            return;
        }

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

    private void SetClearMonsterInfo()
    {
        UpdateMonsterName(-1);
        UpdateMonsterHP(-1);
        UpdateReward(-1);
    }

    public static void ClearMonsterInfo()
    {
        UI_MonsterInfo monsterInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.MonsterInfoUI).GetComponent<UI_MonsterInfo>();
        monsterInfoUI.SetClearMonsterInfo();
    }

    //GameLogicManager의 currentMonsterDataId가 변경될 때 hook으로 불림
    public static void UpdateMonsterInfo(int currentMonsterDataId)
    {
        UI_MonsterInfo monsterInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.MonsterInfoUI).GetComponent<UI_MonsterInfo>();
        monsterInfoUI.SetMonsterInfo(currentMonsterDataId);
    }
}
