using Mirror;
using Mirror.Examples.AdditiveScenes;
using System;
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
    [SyncVar(hook = nameof(CharacterIndexChanged))]
    private int characterIndex = -1;

    [SyncVar(hook = nameof(NicknameChanged))]
    private string nickname;

    [SyncVar(hook = nameof(PlayerColorChanged))]
    private PlayerColor playerColor;

    [Header("Player Models")]
    [SerializeField] List<GameObject> CharacterModels;


    public int CharacterIndex => characterIndex;
    public string Nickname => nickname;
    public PlayerColor PlayerColor => playerColor;

    private void SetCharacterModel(int characterIndex)
    {
        foreach (var characterModel in CharacterModels)
        {
            characterModel.SetActive(false);
        }

        if (characterIndex == -1) return;

        CharacterModels[characterIndex].SetActive(true);
    }


    private void SetNickName(string name)
    {
        //Player 머리위의 닉네임 변경.
    }

    [Server]
    public void InitializeCharacterIndex(NetworkConnectionToClient conn)
    {
        //characterIndex 설정(Room에서 설정한 값)
        int characterIndex = PlayerDataManager.Instance.GetCharacterIndex(conn.identity.netId);
        Debug.Log("characterIndex: " + characterIndex);
        this.characterIndex = characterIndex;
    }

    [Server]
    public void InitializeNickname(NetworkConnectionToClient conn)
    {
        //nickname 설정(Room에서 설정한 값)
        string nickname = PlayerDataManager.Instance.GetNickname(conn.identity.netId);
        this.nickname = nickname;
    }

    [Server]
    public void InitializePlayerColor(int gamePlayerCount)
    {
        //playerColor 설정(정해진 값: 3명이면 RED, YELLOW, BLUE/4~5명이면 PlayerColor 순서대로 할당)
        Array colors = Enum.GetValues(typeof(PlayerColor));
        List<PlayerColor> colorList = new List<PlayerColor>((PlayerColor[])colors);
        if (MyNetworkRoomManager.Instance.roomSlots.Count < 4)
        {
            colorList.Remove(PlayerColor.GREEN);
            colorList.Remove(PlayerColor.PURPLE);
        }

        if (gamePlayerCount > 0 && gamePlayerCount <= colors.Length)
        {
            playerColor = colorList[gamePlayerCount - 1];
        }
        else
        {
            Debug.LogError($"Invalid gamePlayerCount: {gamePlayerCount}");
        }
    }

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

    private void PlayerColorChanged(PlayerColor oldColor, PlayerColor newColor)
    {
        //뭐 없긴함.
    }
    #endregion
}
