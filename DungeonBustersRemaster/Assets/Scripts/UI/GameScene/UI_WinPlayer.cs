using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinPlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Message;
    [SerializeField] Transform Layout_WinPlayer;
    [SerializeField] Button Btn_Confirm;

    [Header("WinPlayerPrefab")]
    [SerializeField] GameObject Prefab_PanelWinPlayer;

    private Dictionary<uint, Panel_WinPlayer> PlayerPanels = new Dictionary<uint, Panel_WinPlayer>();

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void SetPanelInfo(uint winPlayerNetId)
    {
        //이론상 5명의 플레이어가 동점일 수도 있지만, UI상으로는 한꺼번에 3명이 한계임.
        GameObject gObj = Instantiate(Prefab_PanelWinPlayer, Layout_WinPlayer);
        Panel_WinPlayer winPlayer = gObj.GetComponent<Panel_WinPlayer>();
        winPlayer.PlayerNetId = winPlayerNetId;

        if (!PlayerPanels.ContainsKey(winPlayerNetId))
        {
            PlayerPanels[winPlayerNetId] = winPlayer;
        }

        UpdateWinPlayer(winPlayerNetId);
    }

    private void UpdateWinPlayer(uint winPlayerNetId)
    {
        UpdatePlayerNickname(winPlayerNetId);
        UpdatePlayerIcon(winPlayerNetId);
    }

    private void UpdatePlayerNickname(uint winPlayerNetId)
    {
        if (PlayerPanels.ContainsKey(winPlayerNetId))
        {
            PlayerPanels[winPlayerNetId].UpdatePlayerName();
        }
    }

    private void UpdatePlayerIcon(uint winPlayerNetId)
    {
        if(PlayerPanels.ContainsKey(winPlayerNetId))
        {
            PlayerPanels[winPlayerNetId].UpdatePlayerIcon();
        }
    }



    private void UpdateMessage(List<uint> winPlayerNetIds)
    {
        //우승자의 클라이언트
        if(winPlayerNetIds.Contains(NetworkClient.localPlayer.netId))
        {
            Text_Message.text = "우승!\n가장 비열했습니다!";
        }
        else
        {
            Text_Message.text = "가장 눈치 빠르고 영악한\n우승자를 향해 박수를 쳐주세요!";
        }
    }

    private void OnClick_Confirm()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.WinPlayerUI);
        //로비로
    }

    private void ClearPlayerInfo()
    {
        foreach(Transform player in Layout_WinPlayer)
        {
            Destroy(player.gameObject);
        }
    }

    public static void Show(List<uint> winPlayerNetId)
    {
        UIManager.Instance.ShowUI(UIPrefab.WinPlayerUI);
        UI_WinPlayer winPlayerUI = UIManager.Instance.GetActiveUI(UIPrefab.WinPlayerUI).GetComponent<UI_WinPlayer>();

        winPlayerUI.ClearPlayerInfo();

        winPlayerUI.UpdateMessage(winPlayerNetId);

        foreach (uint netId  in winPlayerNetId)
        {
            winPlayerUI.SetPanelInfo(netId);
        }

    }
}
