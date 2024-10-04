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

    public uint PanelNetId
    {
        get { return panelNetId; }
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

    public void UpdatePlayerColor()
    {
        //일단 비워놓고 나중에 Color 추가
    }

    public void UpdatePlayerIcon()
    {
        Img_PlayerIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(playerData.CharacterIndex);
    }

    public void UpdateIsLocal()
    {

    }

    //여기가 진짜
    public void UpdateUsedCards()
    {

        //초기화. 최적화 여지가 있긴함.
        foreach(Transform child in Layout_PlayerUsedCards)
        {
            Destroy(child.gameObject);
        }

        GameObject prefabCard = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Card);

        //SyncList의 정렬은 동기화때문에 꺼려지기 때문에 UI에서 정렬.
        List<int> usedCards = playerGameData.UsedCards;
        usedCards.Sort();

        foreach(int card in usedCards)
        {
            Debug.Log($"<color=red> UsedCard {card} </color>");
        }

        //PlayerColor에 맞는 카드 오브젝트를 UsedCard의 숫자에 맞게 생성
        for (int i = 0; i < usedCards.Count; i++)
        {
            GameObject gObj = Instantiate(prefabCard, Layout_PlayerUsedCards);
            Content_Card usedCard = gObj.GetComponent<Content_Card>();
            usedCard.SetCardNum(usedCards[i]);
            usedCard.SetCardImg(playerData.PlayerColor);
        }
    }

    public void ForceUpdateUsedCards(List<int> tempUsedCards)
    {
        Debug.Log("<color=red>ForceUpdateUsedCards</color>");

        //초기화. 최적화 여지가 있긴함.
        foreach (Transform child in Layout_PlayerUsedCards)
        {
            Destroy(child.gameObject);
        }

        GameObject prefabCard = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Card);

        for (int i = 0; i < tempUsedCards.Count; i++)
        {
            GameObject gObj = Instantiate(prefabCard, Layout_PlayerUsedCards);
            Content_Card usedCard = gObj.GetComponent<Content_Card>();
            usedCard.SetCardNum(tempUsedCards[i]);
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
