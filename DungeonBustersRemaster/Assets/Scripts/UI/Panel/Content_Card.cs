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

    [SerializeField] List<Sprite> cardImgs;

    private Action onClickAction;

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
        onClickAction?.Invoke();
    }

    public void SetCardNum(int cardNum)
    {
        Text_CardNum.text = cardNum.ToString();
    }

    public void SetCardImg(PlayerColor color)
    {
        Img_Card.sprite = cardImgs[(int)color];
    }

    public void SetActiveButton(bool setActive)
    {
        Btn_Card.gameObject.SetActive(setActive);
    }

    public void SetClickAction(Action onClickAction)
    {
        this.onClickAction = onClickAction;
    }
}
