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

    //여기에 필요한거
    //1. 플레이어 정보(플레이어 아이콘(MyPlayer), 플레이어 이름(MyPlayer), 플레이어 색깔(MyPlayer)
    //, 제출한 카드번호(MyPlayerGameData)(이거 SyncVar로 바꿔야겠네?), 공격성공여부(?))
    
    public void SetCharacterIcon(int characterIndex)
    {
        Img_CharacterIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(characterIndex);
    }

    public void SetPlayerName(string nickname)
    {
        Text_PlayerName.text = nickname;
    }

    public void SetSelectedCard(int selectedCardNum, PlayerColor playerColor)
    {
        Img_Card.sprite = SpriteManager.Instance.GetCardSprite(playerColor);
        Text_CardNum.text = selectedCardNum.ToString();
    }

    public void SetAttackSuccess(bool isAttackSuccess)
    {
        Img_X.SetActive(isAttackSuccess);
    }
}
