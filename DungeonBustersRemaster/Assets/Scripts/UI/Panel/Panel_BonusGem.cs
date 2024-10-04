using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_BonusGem : MonoBehaviour
{
    [SerializeField] Button Btn_BonusGem;
    [SerializeField] Image Img_Gem;
    [SerializeField] TextMeshProUGUI Text_GemCount;

    private MyPlayerGameData playerGameData;
    private GemColor bonusColor;

    private Action<GemColor, int> onClickAction;

    private void OnEnable()
    {
        playerGameData = NetworkClient.localPlayer.GetComponent<MyPlayerGameData>();

        onClickAction = null;
        Btn_BonusGem.onClick.AddListener(OnClick_BonusGem);
    }

    private void OnDisable()
    {
        Btn_BonusGem.onClick.RemoveListener(OnClick_BonusGem);
    }

    private void OnClick_BonusGem()
    {
        //로컬플레이어에 Gem 추가해주고, BonusGem을 감소시킴. 아직 BonuGem이 남아있으면 대기화면을 띄운다.
        //ㄴ이런 일을 하는 메서드가 UI_GetBonus에서 SetClickAction으로 구독됨.

        UIManager.Instance.HideUIWithPooling(UIPrefab.GetBonusUI);

        onClickAction?.Invoke(bonusColor, 1);
    }


    public void SetBonusColor(GemColor color)
    {
        bonusColor = color;
        Img_Gem.sprite = SpriteManager.Instance.GetGemSprite(color);

    }

    public void SetBonusCount(int gemCount)
    {
        Text_GemCount.text = $"{gemCount}";
    }

    public void SetClickAction(Action<GemColor, int> onClickAction)
    {
        this.onClickAction += onClickAction;
    }

}
