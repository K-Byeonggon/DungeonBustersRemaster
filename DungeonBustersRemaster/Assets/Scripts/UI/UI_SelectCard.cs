using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectCard : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] Transform Layout_Hands;

    private MyPlayer playerData;
    private MyPlayerGameData playerGameData;

    private void OnEnable()
    {
        playerData = NetworkClient.localPlayer.GetComponent<MyPlayer>();
        playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();

        SetLayoutHands();
        Btn_Background.onClick.AddListener(OnClick_Background);
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
    }


    private void SetLayoutHands()
    {
        //초기화
        foreach (Transform child in Layout_Hands)
        {
            Destroy(child.gameObject);
        }

        //로컬플레이어 정보 받아서 카드 세팅
        GameObject cardPrefab = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Card);

        foreach(int cardNum in playerGameData.Hands)
        {
            GameObject gObj = Instantiate(cardPrefab, Layout_Hands);
            Content_Card card = gObj.GetComponent<Content_Card>();
            card.SetCardImg(playerData.PlayerColor);
            card.SetCardNum(cardNum);
            card.SetActiveButton(true);
            card.SetClickAction(Action_OnClickCard, cardNum);
        }
    }

    private void Action_OnClickCard(int cardNum)
    {
        playerGameData.SubmittedCardNum = cardNum;

        UIManager.Instance.HideUIWithPooling(UIPrefab.SelectCardUI);

        UI_GameScene gameSceneUI = UIManager.Instance.GetActiveUI(UIPrefab.GameSceneUI).GetComponent<UI_GameScene>();
        gameSceneUI.UpdateSelectedCard();
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithPooling(UIPrefab.SelectCardUI);
    }
}
