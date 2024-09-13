using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClientDisconnect : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] Button Btn_Okay;

    private void OnEnable()
    {
        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_Okay.onClick.AddListener(OnClick_Background);
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_Okay.onClick.RemoveListener(OnClick_Background);
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.ClientDisconnectUI);
    }

}
