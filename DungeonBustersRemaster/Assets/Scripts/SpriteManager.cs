using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : Singleton<SpriteManager>
{
    private Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> characterIconSprites = new Dictionary<string, Sprite>();

    public async UniTask LoadAllSprites()
    {
        await LoadAllCards();
        await LoadAllIcons();
    }

    private async UniTask LoadAllCards()
    {
        var cards = Resources.LoadAll<Sprite>("Sprites/Card");
        foreach (var sprite in cards)
        {
            if (sprite != null)
            {
                cardSprites[sprite.name] = sprite;
            }
        }
        await UniTask.Yield();
    }

    private async UniTask LoadAllIcons()
    {
        var icons = Resources.LoadAll<Sprite>("Sprites/CharacterIcon");
        foreach (var sprite in icons)
        {
            if (sprite != null)
            {
                characterIconSprites[sprite.name] = sprite;
            }
        }
        await UniTask.Yield();
    }

    public Sprite GetCardSprite(PlayerColor playerColor)
    {
        string key = playerColor.ToString();
        if (cardSprites.ContainsKey(key))
        {
            return cardSprites[key];
        }
        else
        {
            Debug.LogError($"Invalid PlayerColor: {playerColor}");
            return null;
        }
    }

    public Sprite GetCharacterIconSprite(int characterIndex)
    {
        string key = string.Empty;

        switch (characterIndex)
        {
            case 0:
                key = "WARRIOR";
                break;
            case 1:
                key = "ARCHER";
                break;
            case 2:
                key = "WIZARD";
                break;
        }

        if (characterIconSprites.ContainsKey(key))
        {
            return characterIconSprites[key];
        }
        else
        {
            Debug.LogError($"Invalid characterIndex: {characterIndex}");
            return null;
        }
    }
}
