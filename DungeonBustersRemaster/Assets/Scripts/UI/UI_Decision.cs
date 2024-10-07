using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Decision : MonoBehaviour
{
    [SerializeField] private Button Btn_Background;
    [SerializeField] private TextMeshProUGUI Text_Message;
    [SerializeField] private Button Btn_Yes;
    [SerializeField] private Button Btn_No;

    private Action yesAction;
    private Action noAction;

    private void OnEnable()
    {
        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_Yes.onClick.AddListener(OnClick_Yes);
        Btn_No.onClick.AddListener(OnClick_No);
    }
    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_Yes.onClick.RemoveListener(OnClick_Yes);
        Btn_No.onClick.RemoveListener(OnClick_No);

        yesAction = null;
        noAction = null;
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.DecisionUI);
    }

    private void OnClick_Yes()
    {
        yesAction?.Invoke();
        UIManager.Instance.HideUIWithTimer(UIPrefab.DecisionUI);
    }

    private void OnClick_No()
    {
        noAction?.Invoke();
        UIManager.Instance.HideUIWithTimer(UIPrefab.DecisionUI);
    }

    private void Initialize(string message, Action yesCallback = null, Action noCallback = null)
    {
        Text_Message.text = message;
        yesAction = yesCallback;
        noAction = noCallback;
    }

    public static void Show(string message, Action yesCallback = null, Action noCallback = null)
    {
        UIManager.Instance.ShowUI(UIPrefab.DecisionUI);
        UI_Decision decisionUI = UIManager.Instance.GetActiveUI(UIPrefab.DecisionUI).GetComponent<UI_Decision>();
        decisionUI.Initialize(message, yesCallback, noCallback);
    }
}
