using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Content_Card : MonoBehaviour
{
    [SerializeField] Image Img_Card;
    [SerializeField] Button Btn_Card;
    [SerializeField] TextMeshProUGUI Text_CardNum;

    private Action<int> onClickAction;
    private int actionNum;

    private void OnEnable()
    {
        Btn_Card.onClick.AddListener(OnClick_Card);
    }

    private void OnDisable()
    {
        Btn_Card.onClick.RemoveListener(OnClick_Card);

        onClickAction = null;
    }

    private void OnClick_Card()
    {
        onClickAction?.Invoke(actionNum);
    }

    public void SetCardNum(int cardNum)
    {
        Text_CardNum.text = cardNum.ToString();
    }

    public void SetCardImg(PlayerColor color)
    {
        Img_Card.sprite = SpriteManager.Instance.GetCardSprite(color);
    }

    public void SetActiveButton(bool setActive)
    {
        Btn_Card.enabled = setActive;
    }

    public void SetClickAction(Action<int> onClickAction, int actionNum)
    {
        this.onClickAction = onClickAction;
        this.actionNum = actionNum;
    }
}
