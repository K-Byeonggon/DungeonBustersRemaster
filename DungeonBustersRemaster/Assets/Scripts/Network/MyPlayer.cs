using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] List<GameObject> characterModels;

    [SyncVar(hook = nameof(CharacterIndexChanged))]
    public int characterIndex;

    [SyncVar(hook = nameof(NicknameChanged))]
    public string nickname;


    private void SetCharacterModel(int characterIndex)
    {
        foreach (var characterModel in characterModels)
        {
            characterModel.SetActive(false);
        }

        if (characterIndex == -1) return;

        characterModels[characterIndex].SetActive(true);
    }

    private void SetNickName(string name)
    {
        //Player 머리위의 닉네임 변경.
    }

    #region commands

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
    }

    private void NicknameChanged(string oldName, string newName)
    {
        //Player 머리위의 닉네임 변경.
        SetNickName(newName);
    }
    #endregion
}
