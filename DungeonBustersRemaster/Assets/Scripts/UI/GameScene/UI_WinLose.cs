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

    public void SetWinLoseText()
    {
        //여기서 서버의 전투결과를 가져와서 보여줘야함.
    }

    private void OnClick_Confirm()
    {
        UIManager.Instance.HideUIWithPooling(UIPrefab.WinLoseUI);
        //여기서 UI닫히는거 뿐만 아니라, 서버에 결과를 확인했음을 전송해야함.
    }
}
