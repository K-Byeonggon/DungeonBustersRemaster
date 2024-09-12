using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClientConnect : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] TMP_InputField Input_Address;
    [SerializeField] Button Btn_Connect;

    private MyNetworkRoomManager networkManager;

    private void OnEnable()
    {
        networkManager = FindAnyObjectByType<MyNetworkRoomManager>();
        if(networkManager == null)
        {
            Debug.LogError("MyNetworkRoomManager not found!");
            return;
        }

        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_Connect.onClick.AddListener(OnClick_Connect);
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_Connect.onClick.RemoveListener(OnClick_Connect);
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.ClientConnectUI);
    }

    private void OnClick_Connect()
    {
        string address = Input_Address.text;
        if(string.IsNullOrEmpty(address))
        {
            address = "127.0.0.1";  //defalut
        }

        networkManager.networkAddress = address;
        networkManager.StartClient();
    }
}
