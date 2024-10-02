using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WaitForOther : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Message;

    private void SetMessage(string message)
    {
        Text_Message.text = message;
    }

    public static void Show(string message)
    {
        UIManager.Instance.ShowUI(UIPrefab.WaitForOtherUI);
        UI_WaitForOther waitForOtherUI = UIManager.Instance.GetActiveUI(UIPrefab.WaitForOtherUI).GetComponent<UI_WaitForOther>();
        waitForOtherUI.SetMessage(message);
    }
}
