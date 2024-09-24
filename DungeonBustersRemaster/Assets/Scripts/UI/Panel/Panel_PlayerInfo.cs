using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_PlayerInfo : MonoBehaviour
{
    [SerializeField] Button Btn_Open;
    [SerializeField] Transform Layout_PlayerInfo;
    [SerializeField] GameObject Prefab_PanelGamePlayer;

    private Dictionary<uint, Panel_GamePlayer> PlayerPanels = new Dictionary<uint, Panel_GamePlayer>();

    private void OnEnable()
    {
        Btn_Open.onClick.AddListener(OnClick_Open);
    }

    private void OnDisable()
    {
        Btn_Open.onClick.RemoveListener(OnClick_Open);
    }

    private void OnClick_Open()
    {
        //UI가 미끄러지듯 화면 밖에서 화면안으로 들어옴.
    }

    public void SetPlayerInfo(uint netId)
    {
        GameObject gObj = Instantiate(Prefab_PanelGamePlayer, Layout_PlayerInfo);
        Panel_GamePlayer gamePlayer = gObj.GetComponent<Panel_GamePlayer>();
        gamePlayer.PanelNetId = netId;

        if(!PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId] = gamePlayer;
        }

        InitializePlayerInfo(netId);
    }

    private void InitializePlayerInfo(uint netId)
    {
        UpdatePlayerNickname(netId);
        UpdatePlayerColor(netId);
        UpdatePlayerIcon(netId);
        UpdatePlayerGemsInfo(netId);
        UpdatePlayerUsedCardsInfo(netId);
    }

    //Initialize에만 불리는 것들
    public void UpdatePlayerNickname(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerName();
        }
    }

    public void UpdatePlayerColor(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerColor();
        }
    }

    public void UpdatePlayerIcon(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerIcon();
        }
    }

    //수시로 바뀌는 것들
    public void UpdatePlayerGemsInfo(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdateGems();
        }
    }

    public void UpdatePlayerUsedCardsInfo(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdateUsedCards();
        }
    }

    public void RemovePlayerInfo(uint netId)
    {
        foreach(Transform child in Layout_PlayerInfo)
        {
            Panel_GamePlayer panel = child.gameObject.GetComponent<Panel_GamePlayer>();
            if(panel.PanelNetId == netId)
            {
                Destroy(panel.gameObject);
                return;
            }
        }
    }
}
