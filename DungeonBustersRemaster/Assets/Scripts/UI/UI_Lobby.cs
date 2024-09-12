using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    [Header("Top")]
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] Button Btn_Setting;

    [Header("Mid")]
    [SerializeField] RawImage RImg_Player;

    [Header("Bottom")]
    [SerializeField] Button Btn_StartWithHost;
    [SerializeField] Button Btn_StartWithGuest;
    [SerializeField] Button Btn_SetName;
    [SerializeField] Button Btn_SelectCharacter;

    private MyNetworkRoomManager networkManager;

    private void OnEnable()
    {
        networkManager = FindAnyObjectByType<MyNetworkRoomManager>();
        if(networkManager == null)
        {
            Debug.LogError("MyNetworkRoomManager not found!");
            return;
        }


        Btn_Setting.onClick.AddListener(OnClick_Setting);

        Btn_StartWithHost.onClick.AddListener(OnClick_StartWithHost);
        Btn_StartWithGuest.onClick.AddListener(OnClick_StartWithGuest);
        Btn_SetName.onClick.AddListener(OnClick_SetName);
        Btn_SelectCharacter.onClick.AddListener(OnClick_SelectCharacter);
    }

    private void OnDisable()
    {
        Btn_Setting.onClick.RemoveListener(OnClick_Setting);

        Btn_StartWithHost.onClick.RemoveListener(OnClick_StartWithHost);
        Btn_StartWithGuest.onClick.RemoveListener(OnClick_StartWithGuest);
        Btn_SetName.onClick.RemoveListener(OnClick_SetName);
        Btn_SelectCharacter.onClick.RemoveListener(OnClick_SelectCharacter);
    }




    private void OnClick_Setting()
    {

    }

    private void OnClick_StartWithHost()
    {
        networkManager.StartServer();
    }

    private void OnClick_StartWithGuest()
    {
        UIManager.Instance.ShowUI(UIPrefab.ClientConnectUI);
    }

    private void OnClick_SetName()
    {

    }

    private void OnClick_SelectCharacter()
    {

    }

}
