using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_OpenPlayer : MonoBehaviour
{
    [SerializeField] Image Img_CharacterIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] Image Img_Card;
    [SerializeField] TextMeshProUGUI Text_CardNum;
    [SerializeField] GameObject Img_X;

    private uint panelNetId;
    private MyPlayer playerData;
    private MyPlayerGameData playerGameData;
    
    public uint PanelNetId
    {
        get { return panelNetId;}
        set
        {
            panelNetId = value;
            RegisterPlayerData(panelNetId);
        }
    }

    public void UpdatePlayerName()
    {
        Text_PlayerName.text = playerData.Nickname;
    }

    public void UpdatePlayerIcon()
    {
        Img_CharacterIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(playerData.CharacterIndex);
    }

    public void UpdatePlayerCard()
    {
        Img_Card.sprite = SpriteManager.Instance.GetCardSprite(playerData.PlayerColor);
        Text_CardNum.text = playerGameData.SubmittedCardNum.ToString();
        Img_X.SetActive(!playerGameData.IsAttackSuccess);
    }

    private void RegisterPlayerData(uint netId)
    {
        if(NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            playerData = identity.GetComponent<MyPlayer>();
            playerGameData = identity.GetComponent<MyPlayerGameData>();
        }
    }
}
