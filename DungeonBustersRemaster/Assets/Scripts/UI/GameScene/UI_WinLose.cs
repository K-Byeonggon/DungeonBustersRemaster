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
        UIManager.Instance.HideUIWithPooling(UIPrefab.WinLoseUI);
        //여기서 UI닫히는거 뿐만 아니라, 서버에 결과를 확인했음을 전송해야함.
        //단순하게 하는 방법 GameLogicManager에 컨테이너 만들기
        //거 netId로 Dic만들어서 하기

        //또는 PlayerGameData에 bool변수 만들기 (isStageResultChecked)
        //그거를 Cmd로 쏴서? 그 SubmittedCard처럼 동작
        //아니네 PlayerGameData의 checked를 바꿔도 Server의 LogicManager에서 알아야하네.

        //아무튼 LogicManager에서 알아야함.그러면 컨테이너 써?


        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();
        playerGameData.CmdSendCheckStageResult();
    }

    public static void Show(bool isWin)
    {
        UIManager.Instance.ShowUI(UIPrefab.WinLoseUI);
        UI_WinLose winLoseUI = UIManager.Instance.GetActiveUI(UIPrefab.WinLoseUI).GetComponent<UI_WinLose>();
        winLoseUI.SetWinLoseText(isWin);
    }
}
