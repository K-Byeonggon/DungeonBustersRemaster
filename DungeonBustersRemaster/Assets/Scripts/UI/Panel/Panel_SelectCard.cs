using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_SelectCard : MonoBehaviour
{
    [SerializeField] Button Btn_SelectCard;
    [SerializeField] Button Btn_SubmitCard;
    [SerializeField] Image Img_SelectedCard;
    [SerializeField] TextMeshProUGUI Text_CardNum;

    private void OnEnable()
    {
        Btn_SelectCard.onClick.AddListener(OnClick_SelectCard);
        Btn_SubmitCard.onClick.AddListener(OnClick_SubmitCard);
    }

    private void OnDisable()
    {
        Btn_SelectCard.onClick.RemoveListener(OnClick_SelectCard);
        Btn_SubmitCard.onClick.RemoveListener(OnClick_SubmitCard);
    }

    private void OnClick_SelectCard()
    {
        UIManager.Instance.ShowUI(UIPrefab.SelectCardUI);
    }

    private void OnClick_SubmitCard()
    {

    }

    public void UpdateSelectedCard()
    {
        MyPlayer playerData = NetworkClient.localPlayer.GetComponent<MyPlayer>();
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();

        Img_SelectedCard.sprite = SpriteManager.Instance.GetCardSprite(playerData.PlayerColor);
        Text_CardNum.text = playerGameData.SubmittedCardNum.ToString();
    }

}
