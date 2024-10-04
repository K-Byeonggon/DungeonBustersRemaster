using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GetBonus : MonoBehaviour
{
    [SerializeField] Transform Layout_BonusGems;
    [SerializeField] GameObject bonusPrefab;


    public void SetLayoutBonusGems(List<int> bonusGems)
    {
        foreach(Transform child in Layout_BonusGems)
        {
            Destroy(child.gameObject);
        }

        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        
        for (int index = 0; index < bonusGems.Count; index++)
        {
            if (bonusGems[index] <= 0) { continue; }

            GameObject gObj = Instantiate(bonusPrefab, Layout_BonusGems);
            Panel_BonusGem bonusGem = gObj.GetComponent<Panel_BonusGem>();
            bonusGem.SetBonusColor((GemColor)index);
            bonusGem.SetBonusCount(bonusGems[index]);
            bonusGem.SetClickAction(playerGameData.CmdAddGemsByColor);
            bonusGem.SetClickAction(GameLogicManager.Instance.CmdOnClickPanelBonusGem);
        }

    }


    public static void Show(int[] arrayBonus)
    {
        List<int> bonusGems = new List<int>(arrayBonus);
        Debug.Log("<color=red> UI_GetBonus.Show </color>");
        UIManager.Instance.ShowUI(UIPrefab.GetBonusUI);
        UI_GetBonus getBonusUI = UIManager.Instance.GetActiveUI(UIPrefab.GetBonusUI).GetComponent<UI_GetBonus>();
        getBonusUI.SetLayoutBonusGems(bonusGems);
    }
}
