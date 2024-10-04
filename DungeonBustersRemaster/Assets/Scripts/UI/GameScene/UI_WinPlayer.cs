using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinPlayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Message;
    [SerializeField] Image Img_PlayerIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] Button Btn_Confirm;

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void SetPanelInfo(uint winPlayerNetId)
    {
        MyPlayer playerData = NetworkClient.spawned[winPlayerNetId].GetComponent<MyPlayer>();

        UpdateMessage(winPlayerNetId);
        UpdatePlayerIcon(playerData.CharacterIndex);
        UpdatePlayerName(playerData.Nickname);
    }

    private void UpdatePlayerIcon(int charcaterIndex)
    {
        Img_PlayerIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(charcaterIndex);
    }

    private void UpdatePlayerName(string nickname)
    {
        Text_PlayerName.text = nickname;
    }

    private void UpdateMessage(uint netId)
    {
        //우승자의 클라이언트
        if(NetworkClient.localPlayer.netId == netId)
        {
            Text_Message.text = "우승!\n가장 비열했습니다!";
        }
        else
        {
            Text_Message.text = "가장 눈치 빠르고 영악한\n우승자를 향해 박수를 쳐주세요!";
        }
    }

    private void OnClick_Confirm()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.WinPlayerUI);
        //로비로
    }

    public static void Show(uint winPlayerNetId)
    {
        UIManager.Instance.ShowUI(UIPrefab.WinPlayerUI);
        UI_WinPlayer winPlayerUI = UIManager.Instance.GetActiveUI(UIPrefab.WinPlayerUI).GetComponent<UI_WinPlayer>();

        winPlayerUI.SetPanelInfo(winPlayerNetId);
    }
}
