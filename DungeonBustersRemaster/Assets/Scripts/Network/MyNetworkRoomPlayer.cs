using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    private void Awake()
    {
        Debug.Log("지금");
    }

    public override void OnClientEnterRoom()
    {
        base.OnClientEnterRoom();

        UI_Room roomUI = UIManager.Instance.GetActiveUI(UIPrefab.RoomUI).GetComponent<UI_Room>();
        if (roomUI != null)
        {
            roomUI.ClearLayout();

            //룸 UI의 Layout에 플레이어 채우기
            GameObject roomPlayerUIPrefab = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_RoomPlayer);

            GameObject roomPlayer = Instantiate(roomPlayerUIPrefab, roomUI.Layout_RoomPlayers);

            UI_RoomPlayer roomPlayerUI = roomPlayer.GetComponent<UI_RoomPlayer>();

            roomPlayerUI.SetPlayerInfo($"Player{index+1}", readyToBegin);
        }
    }
}
