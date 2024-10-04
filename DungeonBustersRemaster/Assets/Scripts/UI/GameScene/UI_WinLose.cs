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
    [SerializeField] Button Btn_Confirm;

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void PlayWinLoseAnimation()
    {
        //할 수 있다면 오버워치 승리/패배처럼 결과창 애니메이션을 추가하고 싶음.
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


        /*
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        playerGameData.CmdSendCheckStageResult();
        */
    }

    public static void Show(bool isWin)
    {
        UIManager.Instance.ShowUI(UIPrefab.WinLoseUI);
        UI_WinLose winLoseUI = UIManager.Instance.GetActiveUI(UIPrefab.WinLoseUI).GetComponent<UI_WinLose>();
        winLoseUI.SetWinLoseText(isWin);
    }
}
