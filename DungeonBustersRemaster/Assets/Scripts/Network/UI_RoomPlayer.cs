using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomPlayer : NetworkRoomPlayer
{
    [SerializeField] Image Img_Player;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] Image Img_Ready;

}
