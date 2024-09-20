using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField] List<GameObject> characterModels;

    [SyncVar(hook = nameof(CharacterIndexChanged))]
    public int characterIndex;


    private void SetCharacterModel(int characterIndex)
    {
        foreach (var characterModel in characterModels)
        {
            characterModel.SetActive(false);
        }

        if (characterIndex == -1) return;

        characterModels[characterIndex].SetActive(true);
    }

    #region commands

    [Command]
    public void CmdChangeCharacterIndex(int index)
    {
        characterIndex = index;
    }

    #endregion

    #region hook

    private void CharacterIndexChanged(int oldIndex, int newIndex)
    {
        Debug.Log($"{oldIndex} -> {newIndex}");

        //플레이어 모델링 바꾸기
        SetCharacterModel(newIndex);
    }
    #endregion
}
