using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BonusGems : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> Text_BonusGems;


    private void SetBonusInfo(List<int> bonusGems)
    {
        for (int i = 0; i < bonusGems.Count; i++)
        {
            Text_BonusGems[i].text = bonusGems[i].ToString();
        }
    }

    //GameLogicManager의 bonusGems가 변경될 때 hook으로 불림
    public static void UpdateBonusGems(List<int> newBonusGems)
    {
        UI_BonusGems bonusUI = UIManager.Instance.GetActiveUI(UIPrefab.BonusGemsUI).GetComponent<UI_BonusGems>();
        if(bonusUI != null)
        {
            bonusUI.SetBonusInfo(newBonusGems);
        }
    }
}
