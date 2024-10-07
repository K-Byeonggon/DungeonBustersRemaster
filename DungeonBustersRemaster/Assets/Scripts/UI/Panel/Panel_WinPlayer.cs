using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_WinPlayer : MonoBehaviour
{
    [SerializeField] Image Img_PlayerIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;

    private uint panelNetId;
    private MyPlayer playerData;

    public uint PlayerNetId
    {
        get { return panelNetId;}
        set
        {
            panelNetId = value;
            RegistPlayerData(panelNetId);
        }
    }

    public void UpdatePlayerName()
    {
        Text_PlayerName.text = playerData.Nickname;
    }

    public void UpdatePlayerIcon()
    {
        Img_PlayerIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(playerData.CharacterIndex);
    }


    private void RegistPlayerData(uint netId)
    {
        if(NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            playerData = identity.GetComponent<MyPlayer>();
        }
    }
}
