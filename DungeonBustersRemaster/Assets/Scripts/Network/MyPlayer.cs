using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerColor
{
    RED,
    GREEN,
    YELLOW,
    BLUE,
    PURPLE
}

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] List<GameObject> CharacterModels;
    [SerializeField] List<Sprite> Sprite_CharacterIcon;
    private Image Img_CharacterIcon;
    private PlayerColor PlayerColor;


    [SyncVar(hook = nameof(CharacterIndexChanged))]
    public int characterIndex;

    [SyncVar(hook = nameof(NicknameChanged))]
    public string nickname;


    private void SetCharacterModel(int characterIndex)
    {
        foreach (var characterModel in CharacterModels)
        {
            characterModel.SetActive(false);
        }

        if (characterIndex == -1) return;

        CharacterModels[characterIndex].SetActive(true);
    }

    private void SetCharacterIcon(int characterIndex)
    {
        Img_CharacterIcon.sprite = Sprite_CharacterIcon[characterIndex];
    }

    private void SetNickName(string name)
    {
        //Player 머리위의 닉네임 변경.
    }

    #region commands

    //GamePlayer생성 후 불린다.
    [Command]
    public void CmdChangeCharacterIndex(int index)
    {
        characterIndex = index;
    }

    [Command]
    public void CmdChangeNickname(string name)
    {
        nickname = name;
    }

    #endregion

    #region hook

    private void CharacterIndexChanged(int oldIndex, int newIndex)
    {
        Debug.Log($"{oldIndex} -> {newIndex}");

        //플레이어 모델링 바꾸기
        SetCharacterModel(newIndex);
        SetCharacterIcon(newIndex);
    }

    private void NicknameChanged(string oldName, string newName)
    {
        //Player 머리위의 닉네임 변경.
        SetNickName(newName);
    }
    #endregion
}
