using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GetBonus : MonoBehaviour
{
    [SerializeField] Transform Layout_BonusGems;

    public void SetLayoutBonusGems(GameLogicManager manager)
    {
        foreach(Transform child in Layout_BonusGems)
        {
            Destroy(child.gameObject);
        }

        List<int> bonusGems = manager.BonusGems;
        GameObject bonusPrefab = null;
        
        for (int index = 0; index < bonusGems.Count; index++)
        {
            if (bonusGems[index] <= 0) { continue; }

            GameObject gObj = Instantiate(bonusPrefab, Layout_BonusGems);
            Panel_BonusGem bonusGem = gObj.GetComponent<Panel_BonusGem>();
            bonusGem.SetBonusColor((GemColor)index);
            bonusGem.SetBonusCount(bonusGems[index]);
            bonusGem.SetClickAction(manager.CmdSubBonusGems);
        }

    }

    public static void UpdateGetBonusGems(GameLogicManager manager)
    {
        
        UIManager.Instance.HideUIWithPooling(UIPrefab.WaitForOtherUI);

        UI_GetBonus getBonusUI = UIManager.Instance.GetActiveUI(UIPrefab.GetBonusUI).GetComponent<UI_GetBonus>();
        getBonusUI.SetLayoutBonusGems(manager);
        //manager의 Cmd메서드를 Layout의 컨텐츠의 Action에 붙이기. Server의 GameLogicManager의 bonesGems 수정하
    }
}
