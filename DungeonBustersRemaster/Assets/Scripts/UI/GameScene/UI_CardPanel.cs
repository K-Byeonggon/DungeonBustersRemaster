using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CardPanel : MonoBehaviour
{
    [SerializeField] Button Btn_SelectCard;
    [SerializeField] Button Btn_SubmitCard;
    [SerializeField] TextMeshProUGUI Text_SubmitCard;
    [SerializeField] Image Img_SelectedCard;
    [SerializeField] Image Img_Submitted;
    [SerializeField] TextMeshProUGUI Text_CardNum;

    private bool isSubmit = true;

    private void OnEnable()
    {
        Btn_SelectCard.onClick.AddListener(OnClick_SelectCard);
        Btn_SubmitCard.onClick.AddListener(OnClick_SubmitCard);
        Initialize();
    }

    private void OnDisable()
    {
        Btn_SelectCard.onClick.RemoveListener(OnClick_SelectCard);
        Btn_SubmitCard.onClick.RemoveListener(OnClick_SubmitCard);
    }

    public void Initialize()
    {
        isSubmit = true;
        Text_SubmitCard.text = "카드 제출";
        Img_Submitted.gameObject.SetActive(false);
        Text_CardNum.text = string.Empty;
    }

    private void OnClick_SelectCard()
    {
        UIManager.Instance.ShowUI(UIPrefab.SelectCardUI);
    }

    private void OnClick_SubmitCard()
    {
        //어떻게 제출 여부를 GameLogicManager에 알리나?
        //그냥 GameLogicManager에 SubmittedPlayerNum을 증가시키고 플레이어 수와 알맞게 되면 진행하는게?
        if (isSubmit)
        {
            SubmitCard();
        }
        else
        {
            CancelCard();
        }

    }

    private void SubmitCard()
    {
        GameLogicManager.Instance.CmdAddSubmittedPlayerCount();
        Text_SubmitCard.text = "제출 취소";
        Img_Submitted.gameObject.SetActive(true);
        isSubmit = false;
    }

    private void CancelCard()
    {
        GameLogicManager.Instance.CmdSubSubmittedPlayerCount();
        Text_SubmitCard.text = "카드 제출";
        Img_Submitted.gameObject.SetActive(false);
        isSubmit = true;
    }


    public void UpdateSelectedCard()
    {
        MyPlayer playerData = NetworkClient.localPlayer.GetComponent<MyPlayer>();
        MyPlayerGameData playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();

        Img_SelectedCard.sprite = SpriteManager.Instance.GetCardSprite(playerData.PlayerColor);
        Text_CardNum.text = playerGameData.SubmittedCardNum != 0 ? playerGameData.SubmittedCardNum.ToString() : string.Empty;
    }

}
