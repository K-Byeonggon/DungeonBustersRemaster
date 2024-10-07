using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_LoseGems : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> Text_BonusGemCounts;
    [SerializeField] Transform Layout_Gems;

    [Header("Prefab")]
    [SerializeField] GameObject Prefab_PanelLoseGem;

    private MyPlayerGameData playerGameData;


    private void OnEnable()
    {
        playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();



        SetBonusInfo(GameLogicManager.Instance.BonusGems);
        SetLayoutGems();
        

    }

    private void OnDisable()
    {
        
    }


    private void SetLayoutGems()
    {
        foreach (Transform child in Layout_Gems)
        {
            Destroy(child.gameObject);
        }

        //playerGameData에서 가장 많은 보석의 index를 구한다.
        int maxValue = playerGameData.Gems.Max();
        List<int> maxGems = playerGameData.Gems
            .Select((value, index) => new { value, index })
            .Where(x => x.value == maxValue)
            .Select(x => x.index)
            .ToList();

        foreach(int gemIndex in maxGems)
        {
            GameObject gObj = Instantiate(Prefab_PanelLoseGem, Layout_Gems);
            Panel_LoseGem loseGem = gObj.GetComponent<Panel_LoseGem>();
            loseGem.Initialize((GemColor)gemIndex, playerGameData.Gems[gemIndex], Action_OnClickGem);
        }
    }

    private void SetBonusInfo(List<int> bonusGems)
    {
        for (int i = 0; i < bonusGems.Count; i++)
        {
            Text_BonusGemCounts[i].text = bonusGems[i].ToString();
        }
    }


    private void Action_OnClickGem(GemColor color)
    {
        //해당 보석 모두 잃는 Cmd메서드
        playerGameData.CmdLoseAllGemsByColor(color);

        //보석 제출 했다고 logicManager에 알리기
        //GameLogicManager.Instance.RegisterLoseGemResultChecked(NetworkClient.localPlayer.netId);
        GameLogicManager.Instance.CmdCheckConfirm(NetworkClient.localPlayer.netId, ConfirmPhase.LoseGemsResult);

        //UI변경 (대기)
        UIManager.Instance.HideUIWithPooling(UIPrefab.LoseGemsUI);
        UI_WaitForOther.Show("다른 플레이어의 선택을 기다리고 있습니다.");
    }

    //GameLogicManager의 BonusGems가 바뀌면 불리는 걸로
    public static void UpdateBonusGems(List<int> newBonusGems)
    {
        UI_LoseGems loseGemsUI = UIManager.Instance.GetActiveUI(UIPrefab.LoseGemsUI).GetComponent<UI_LoseGems>();

        loseGemsUI.SetBonusInfo(newBonusGems);

    }
}
