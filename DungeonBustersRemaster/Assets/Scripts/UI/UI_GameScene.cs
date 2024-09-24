using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : MonoBehaviour
{
    [SerializeField] Button Btn_Config;

    [SerializeField] Panel_MonsterInfo Panel_MonsterInfo;
    [SerializeField] Panel_PlayerInfo Panel_PlayerInfo;
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


    #region Panel_PlayerInfo

    //MyPlayerGameData가 OnStartClient 할때 실행됨.
    public void SetPlayerInfo(uint netId)
    {
        Panel_PlayerInfo.SetPlayerInfo(netId);
    }

    //MyPlayerGameData의 hook에서 실행됨.
    public void UpdatePlayerGemsInfo(uint netId)
    {
        Panel_PlayerInfo.UpdatePlayerGemsInfo(netId);
    }

    public void UpdatePlayerUsedCardInfo(uint netId)
    {
        Panel_PlayerInfo.UpdatePlayerUsedCardsInfo(netId);
    }

    //MyPlayerGameData가 OnStopClient 할때 실행됨.
    public void RemovePlayerInfo(uint netId)
    {
        Panel_PlayerInfo.RemovePlayerInfo(netId);
    }
    #endregion

    #region Panel_SelectCard

    public void UpdateSelectedCard()
    {
        Panel_SelectCard.UpdateSelectedCard();
    }
    #endregion
}
