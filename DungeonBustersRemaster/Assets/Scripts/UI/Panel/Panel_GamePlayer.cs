using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_GamePlayer : MonoBehaviour
{
    private uint panelNetId;
    private MyPlayer playerData;
    private MyPlayerGameData playerGameData;

    [SerializeField] Image Img_IsLocal;
    [SerializeField] Image Img_PlayerColor;
    [SerializeField] Image Img_PlayerIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] List<TextMeshProUGUI> PlayerGems;
    [SerializeField] Transform Layout_PlayerUsedCards;

    [Header("Card Prefab")]
    [SerializeField] GameObject Prefab_ContentCard;

    [Header("Character Sprites")]
    [SerializeField] List<Sprite> CharacterSprites;

    public uint PanelNetId
    {
        get { return panelNetId; }
        set
        {
            panelNetId = value;
            RegistPlayerData(panelNetId);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void UpdatePlayerName()
    {
        Text_PlayerName.text = playerData.Nickname;
    }

    public void UpdatePlayerColor()
    {
        //일단 비워놓고 나중에 Color 추가
    }

    public void UpdatePlayerIcon()
    {
        Img_PlayerIcon.sprite = CharacterSprites[playerData.CharacterIndex];
    }

    public void UpdateIsLocal()
    {

    }

    //여기가 진짜
    public void UpdateUsedCards()
    {
        //초기화. 최적화 여지가 있긴함.
        foreach(GameObject child in Layout_PlayerUsedCards)
        {
            Destroy(child);
        }

        //PlayerColor에 맞는 카드 오브젝트를 UsedCard의 숫자에 맞게 생성
        for (int i = 0; i < playerGameData.UsedCards.Count; i++)
        {
            GameObject gObj = Instantiate(Prefab_ContentCard, Layout_PlayerUsedCards);
            Content_Card usedCard = gObj.GetComponent<Content_Card>();
            usedCard.SetCardNum(playerGameData.UsedCards[i]);
            usedCard.SetCardImg(playerData.PlayerColor);
        }
    }

    public void UpdateGems()
    {
        //MyPlayerGameData의 gems
        for (int i = 0; i < PlayerGems.Count; i++)
        {
            PlayerGems[i].text = $"{playerGameData.Gems[i]}";
        }
    }


    //PanelNetId가 set 될때 netId로 MyPlayerGameData등록
    private void RegistPlayerData(uint netId)
    {
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            playerData = identity.GetComponent<MyPlayer>();
            playerGameData = identity.GetComponent<MyPlayerGameData>();
        }
    }
}
