using Mirror;
using Mirror.Examples.CharacterSelection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinLose : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_WinLose;
    [SerializeField] Transform Layout_OpenPlayer;
    [SerializeField] Button Btn_Confirm;

    [Header("OpenPlayerPrefab")]
    [SerializeField] GameObject Prefab_PanelResult;

    private Dictionary<uint, Panel_OpenPlayer> PlayerPanels = new Dictionary<uint, Panel_OpenPlayer>();

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
        foreach(Transform panel in Layout_OpenPlayer)
        {
            Destroy(panel.gameObject);
        }
    }

    private void SetOpenPlayerInfo(uint netId)
    {
        GameObject gObj = Instantiate(Prefab_PanelResult, Layout_OpenPlayer);
        Panel_OpenPlayer openPlayer = gObj.GetComponent<Panel_OpenPlayer>();
        openPlayer.PanelNetId = netId;

        if (!PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId] = openPlayer;
        }

        UpdateOpenPlayer(netId);
    }

    private void UpdateOpenPlayer(uint netId)
    {
        UpdatePlayerNickName(netId);
        UpdatePlayerIcon(netId);
        UpdatePlayerCard(netId);        
    }

    private void UpdatePlayerNickName(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
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
    private void UpdatePlayerCard(uint netId)
    {
        if (PlayerPanels.ContainsKey(netId))
        {
            PlayerPanels[netId].UpdatePlayerCard();
        }
    }



    private void SetWinLoseText(bool isWin)
    {

        if (isWin)
        {
            Text_WinLose.text = "토벌 성공!";
        }
        else
        {
            Text_WinLose.text = "토벌 실패..";
        }
    }

    private void OnClick_Confirm()
    {
        //LogicManager에 확인했다고 결과 전송
        GameLogicManager.Instance.CmdCheckConfirm(NetworkClient.localPlayer.netId, ConfirmPhase.StageBattleResult);

        UIManager.Instance.HideUIWithPooling(UIPrefab.WinLoseUI);
        UI_WaitForOther.Show("다른 플레이어의 결과 확인을 기다리는 중..");
    }

    public static void Show(bool isWin)
    {
        UIManager.Instance.ShowUI(UIPrefab.WinLoseUI);
        UI_WinLose winLoseUI = UIManager.Instance.GetActiveUI(UIPrefab.WinLoseUI).GetComponent<UI_WinLose>();

        winLoseUI.ClearPlayerInfo();

        foreach(var kvp in NetworkClient.spawned)
        {
            if(kvp.Value.TryGetComponent(out MyPlayer player))
            {
                winLoseUI.SetOpenPlayerInfo(kvp.Key);
            }
        }

        winLoseUI.SetWinLoseText(isWin);
    }
}
