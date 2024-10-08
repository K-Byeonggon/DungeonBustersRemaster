using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SetPlayerNum : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] Button Btn_3Players;
    [SerializeField] Button Btn_45Players;


    private void OnEnable()
    {
        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_3Players.onClick.AddListener(OnClick_3Players);
        Btn_45Players.onClick.AddListener(OnClick_45Players);
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_3Players.onClick.RemoveListener(OnClick_3Players);
        Btn_45Players.onClick.RemoveListener(OnClick_45Players);
    }



    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.SetPlayerNumUI);
    }

    private void OnClick_3Players()
    {
        //MyNetworkRoomManager.Instance.minPlayers = 3;
        MyNetworkRoomManager.Instance.StartHost();
    }

    private void OnClick_45Players()
    {
        MyNetworkRoomManager.Instance.minPlayers = 4;
        MyNetworkRoomManager.Instance.StartHost();
    }

}
