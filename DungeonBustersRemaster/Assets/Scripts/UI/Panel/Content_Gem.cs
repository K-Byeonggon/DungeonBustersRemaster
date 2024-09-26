using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GemColor
{
    RED,
    YELLOW,
    BLUE
}

public class Content_Gem : MonoBehaviour
{
    [SerializeField] Image Img_Gem;

    public void SetGemImg(GemColor color)
    {
        Img_Gem.sprite = SpriteManager.Instance.GetGemSprite(color);
    }
}
