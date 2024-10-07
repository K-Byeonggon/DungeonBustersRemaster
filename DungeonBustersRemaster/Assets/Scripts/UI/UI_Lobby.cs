using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] Button Btn_Setting;

    [Header("Bottom")]
    [SerializeField] Button Btn_StartWithHost;
    [SerializeField] Button Btn_StartWithGuest;
    [SerializeField] Button Btn_Quit;


    private void OnEnable()
    {


        Btn_Setting.onClick.AddListener(OnClick_Setting);

        Btn_StartWithHost.onClick.AddListener(OnClick_StartWithHost);
        Btn_StartWithGuest.onClick.AddListener(OnClick_StartWithGuest);
        Btn_Quit.onClick.AddListener(OnClick_Quit);
    }

    private void OnDisable()
    {
        Btn_Setting.onClick.RemoveListener(OnClick_Setting);

        Btn_StartWithHost.onClick.RemoveListener(OnClick_StartWithHost);
        Btn_StartWithGuest.onClick.RemoveListener(OnClick_StartWithGuest);
        Btn_Quit.onClick.RemoveListener(OnClick_Quit);
    }




    private void OnClick_Setting()
    {

    }

    private void OnClick_StartWithHost()
    {
        UIManager.Instance.ShowUI(UIPrefab.SetPlayerNumUI);
    }

    private void OnClick_StartWithGuest()
    {
        UIManager.Instance.ShowUI(UIPrefab.ClientConnectUI);
    }

    private void OnClick_Quit()
    {
        string message = "게임을 종료하시겠습니까?";
        UI_Decision.Show(message, QuitGame);
    }

    private void QuitGame()
    {
        Debug.Log("QuitGame");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
