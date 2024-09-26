using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : MonoBehaviour
{
    [SerializeField] GameObject Img_Overlay;
    [SerializeField] RectTransform Panel_PlayerInfo;
    [SerializeField] Button Btn_Open;
    [SerializeField] TextMeshProUGUI Text_OpenBtn;
    [SerializeField] Transform Layout_PlayerInfo;

    [Header("GamePlayerPrefab")]
    [SerializeField] GameObject Prefab_PanelGamePlayer;

    private bool IsOpened;

    private const float OpenDuration = 0.8f;
 
    private Dictionary<uint, Panel_GamePlayer> PlayerPanels = new Dictionary<uint, Panel_GamePlayer>();

    private void OnEnable()
    {
        IsOpened = false;
        Img_Overlay.SetActive(IsOpened);
        Btn_Open.onClick.AddListener(OnClick_Open);
    }

    private void OnDisable()
    {
        Btn_Open.onClick.RemoveListener(OnClick_Open);
    }

    private void OnClick_Open()
    {
        IsOpened = !IsOpened;
        Img_Overlay.SetActive(IsOpened);
        MovePanel();
    }

    private void MovePanel()
    {
        if(IsOpened)
        {
            Panel_PlayerInfo.DOAnchorPos(new Vector2(0f, 0f), OpenDuration).SetEase(Ease.OutQuad);
            Text_OpenBtn.text = ">";
        }
        else
        {
            Panel_PlayerInfo.DOAnchorPos(new Vector2(640f, 0f), OpenDuration).SetEase(Ease.OutQuad);
            Text_OpenBtn.text = "<";
        }
    }

    //OnStartClient에서 불린다.
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

    //OnStopClient에서 불린다
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
