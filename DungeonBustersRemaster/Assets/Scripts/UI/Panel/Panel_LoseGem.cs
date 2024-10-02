using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_LoseGem : MonoBehaviour
{
    [SerializeField] Image Img_Gem;
    [SerializeField] TextMeshProUGUI Text_GemCount;
    [SerializeField] Button Btn_LoseGem;

    private Action<GemColor> onClickAction;
    private GemColor actionColor;

    private void OnEnable()
    {
        Btn_LoseGem.onClick.AddListener(OnClick_LoseGem);
    }

    private void OnDisable()
    {
        Btn_LoseGem.onClick.RemoveListener(OnClick_LoseGem);
        onClickAction = null;
    }

    public void Initialize(GemColor color, int count, Action<GemColor> onClickAction)
    {
        Img_Gem.sprite = SpriteManager.Instance.GetGemSprite(color);
        Text_GemCount.text = count.ToString();
        actionColor = color;
        this.onClickAction = onClickAction;
    }

    private void OnClick_LoseGem()
    {
        onClickAction?.Invoke(actionColor);
    }
}
