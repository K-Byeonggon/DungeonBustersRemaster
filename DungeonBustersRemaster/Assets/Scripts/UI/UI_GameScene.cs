using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : MonoBehaviour
{
    [SerializeField] Button Btn_Config;

    [SerializeField] Panel_MonsterInfo Panel_MonsterInfo;
    [SerializeField] Panel_SelectCard Panel_SelectCard;

    private void OnEnable()
    {
        Btn_Config.onClick.AddListener(OnClick_Config);
    }

    private void OnDisable()
    {
        Btn_Config.onClick.RemoveListener(OnClick_Config);
    }

    private void OnClick_Config()
    {

    }

    public void UpdateMonsterInfo()
    {
        //외부에서 UIManager를 통해 불려서 Panel을 수정하도록.
        Panel_MonsterInfo.UpdateMonsterInfo();
    }


    #region Panel_SelectCard

    public void UpdateSelectedCard()
    {
        Panel_SelectCard.UpdateSelectedCard();
    }
    #endregion
}
