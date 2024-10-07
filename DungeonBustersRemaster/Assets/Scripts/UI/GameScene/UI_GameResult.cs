using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameResult : MonoBehaviour
{
    [SerializeField] Transform Layout_Players;
    [SerializeField] Button Btn_Confirm;

    [Header("ResultPrefab")]
    [SerializeField] GameObject Prefab_PanelResult;

    private Dictionary<uint, Panel_Result> PlayerPanels = new Dictionary<uint, Panel_Result>();

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    
    private void ClearPlayerInfo()
    {
        PlayerPanels.Clear();
        foreach(Transform panel in Layout_Players)
        {
            Destroy(panel.gameObject);
        }
    }

    private void SetPlayerInfo(uint netId)
    {
        GameObject gObj = Instantiate(Prefab_PanelResult, Layout_Players);
        Panel_Result playerResult = gObj.GetComponent<Panel_Result>();
        playerResult.PanelNetId = netId;

        if (!PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId] = playerResult;
        }

        UpdatePlayerResult(netId);
    }

    private void UpdatePlayerResult(uint netId)
    {
        UpdatePlayerNickName(netId);
        UpdatePlayerIcon(netId);
        UpdatePlayerGemsInfo(netId);
        UpdatePlayerTotalPoint(netId);
    }

    private void UpdatePlayerNickName(uint netId)
    {
        if(PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerName();
        }
    }

    private void UpdatePlayerIcon(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerIcon();
        }
    }

    private void UpdatePlayerGemsInfo(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdateGemCounts();
            PlayerPanels[netId].UpdateGemSetCount();
        }
    }

    private void UpdatePlayerTotalPoint(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdateTotalPoint();
        }
    }

    private void OnClick_Confirm()
    {
        //우승자 띄우는 UI로
        //이부분은 다른 플레이어를 기다릴 필요가 없다.
        //클라별로 그냥 갱신. 그러면 TargetRpc를 써야겠군.

        MyPlayer player = NetworkClient.localPlayer.GetComponent<MyPlayer>();

        player.CmdRequestSetPhaseGameEnd();

        UIManager.Instance.HideUIWithTimer(UIPrefab.GameResultUI);

    }

    public static void Show()
    {
        UIManager.Instance.ShowUI(UIPrefab.GameResultUI);
        UI_GameResult gameResultUI = UIManager.Instance.GetActiveUI(UIPrefab.GameResultUI).GetComponent<UI_GameResult>();

        gameResultUI.ClearPlayerInfo();

        //spawned의 Player만 세어서 netId로 SetPlayerInfo 반복.
        foreach(var kvp in NetworkClient.spawned)
        {
            if(kvp.Value.TryGetComponent(out MyPlayer player))
            {
                gameResultUI.SetPlayerInfo(kvp.Key);
            }
        }
    }
}
