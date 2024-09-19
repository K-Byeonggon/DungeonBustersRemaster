using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Notify : MonoBehaviour
{
    [SerializeField] private Button Btn_Background;
    [SerializeField] private TextMeshProUGUI Text_Message;
    [SerializeField] private Button Btn_Confirm;
    [SerializeField] private TextMeshProUGUI Text_Confirm;

    private Action confirmAction;

    private void OnEnable()
    {
        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);

        confirmAction = null;
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.NotifyUI);
    }

    private void OnClick_Confirm()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.NotifyUI);
    }

    public void Initialize(string message, string confirm, Action confirmCallback = null)
    {
        Text_Message.text = message;
        Text_Confirm.text = confirm;
        confirmAction = confirmCallback;
    }

    public static void Show(string message, string confirm = "확인" , Action confirmCallback = null)
    {
        UIManager.Instance.ShowUI(UIPrefab.NotifyUI);
        UI_Notify notifyUI = UIManager.Instance.GetActiveUI(UIPrefab.NotifyUI).GetComponent<UI_Notify>();
        notifyUI.Initialize(message, confirm, confirmCallback);
    }
}
