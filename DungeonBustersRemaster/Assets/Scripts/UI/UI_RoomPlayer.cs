using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomPlayer : MonoBehaviour
{
    [SerializeField] Image Img_Player;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] Image Img_Ready;

    [SerializeField] List<Sprite> Sprites_Character;

    public void SetPlayerInfo(string playerName, bool isReady, int characterIndex)
    {
        Text_PlayerName.text = playerName;
        Img_Ready.gameObject.SetActive(isReady);
        Img_Player.sprite = Sprites_Character[characterIndex];
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
