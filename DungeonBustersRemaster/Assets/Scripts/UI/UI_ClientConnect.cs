using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClientConnect : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] TMP_InputField Input_Address;
    [SerializeField] TextMeshProUGUI Text_Error;
    [SerializeField] Button Btn_Connect;


    private void OnEnable()
    {
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

        MyNetworkRoomManager.Instance.networkAddress = address;

        if(!IsValidAddress(address))
        {
            Text_Error.text = "잘못된 주소입니다.\n다시 입력해 주세요.";
            return;
        }
        else
        {
            Text_Error.text = "주소는 주소에용";

        }

        try
        {
            MyNetworkRoomManager.Instance.StartClient();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"클라이언트 시작 중 오류 발생: {e.Message}");
            Text_Error.text = "클라이언트 시작 중 오류가 발생했습니다.";
        }
    }

    private bool IsValidAddress(string address)
    {
        System.Net.IPAddress ip;
        return System.Net.IPAddress.TryParse(address, out ip);
    }

}
