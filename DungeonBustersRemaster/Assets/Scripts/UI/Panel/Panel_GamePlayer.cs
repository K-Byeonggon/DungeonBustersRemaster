using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_GamePlayer : MonoBehaviour
{
    [SerializeField] Image Img_IsLocal;
    [SerializeField] Image Img_PlayerColor;
    [SerializeField] Image Img_PlayerIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] List<int> PlayerGems;
    [SerializeField] Transform Layout_PlayerUsedCards;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void UpdatePlayerName(string playerName)
    {

    }

    public void UpdatePlayerColor(PlayerColor playerColor)
    {

    }

    public void UpdatePlayerIcon(int characterIndex)
    {

    }

    public void UpdateIsLocal()
    {

    }

    //여기가 진짜
    public void UpdateUsedCards()
    {
        //PlayerColor에 맞는 카드 오브젝트를 UsedCard의 숫자에 맞게 생성
    }

    public void UpdateGems()
    {
        //MyPlayerGameData의 gems
        //MyNetworkRoomManager.Instance.roomSlots
    }
}
