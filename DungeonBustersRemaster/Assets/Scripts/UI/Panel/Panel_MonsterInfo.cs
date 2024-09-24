using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Panel_MonsterInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_MonsterName;
    [SerializeField] TextMeshProUGUI Text_MonsterHP;
    [SerializeField] Transform Layout_Reward;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    //UI_GameScene의 UpdateMonsterInfo에서 불림
    public void UpdateMonsterInfo()
    {
        UpdateMonsterName();
        UpdateMonsterHP();
        UpdtateReward();
    }

    private void UpdateMonsterName()
    {
        Text_MonsterName.text = "몬스터 이름";
    }

    private void UpdateMonsterHP()
    {
        Text_MonsterHP.text = "HP: 1234";
    }

    private void UpdtateReward()
    {
        //이게 좀 어렵겠군
    }
}
