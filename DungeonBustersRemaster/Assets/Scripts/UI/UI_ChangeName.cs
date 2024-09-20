using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Adjective
{
    Brave,
    Elite,
    Evil,
    Holy
}
public enum Noun
{
    Cat,
    Moly,
    Cookie,
    Trainer
}

public class UI_ChangeName : MonoBehaviour
{
    [SerializeField] private Button Btn_Background;
    [SerializeField] private TextMeshProUGUI Text_Name;
    [SerializeField] private Button Btn_Confirm;
    [SerializeField] private Button Btn_Reroll;

    private string name_Adjective;
    private string name_Noun;
    private int adjLength;
    private int nounLength;
    private string newName;


    private void OnEnable()
    {
        Initialize();

        Btn_Background.onClick.AddListener(OnClick_Background);
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
        Btn_Reroll.onClick.AddListener(OnClick_Reroll);
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
        Btn_Reroll.onClick.RemoveListener(OnClick_Reroll);
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.ChangeNameUI);
    }

    private void OnClick_Confirm()
    {
        //RoomPlayer의 Name 변경(아무튼 characterIndex처럼 동작)
        NetworkRoomPlayer roomPlayer = MyNetworkRoomManager.Instance.GetLocalRoomPlayer();
        MyNetworkRoomPlayer myRoomPlayer = roomPlayer.GetComponent<MyNetworkRoomPlayer>();

        //newName검사
        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = $"Player{roomPlayer.index + 1}";
        }

        myRoomPlayer.CmdChangeNickName(newName);

        UIManager.Instance.HideUIWithTimer(UIPrefab.ChangeNameUI);
    }

    private void OnClick_Reroll()
    {
        newName = GetNewName();
        Text_Name.text = newName;
    }


    private void Initialize()
    {
        adjLength = Enum.GetValues(typeof(Adjective)).Length;
        nounLength = Enum.GetValues(typeof(Noun)).Length;
    }

    private string GetNewName()
    {
        //새 닉네임 얻는 로직(나중에는 xml등으로 저장된 이름 사용)

        int adjNum = UnityEngine.Random.Range(0, adjLength);
        int nounNum = UnityEngine.Random.Range(0, nounLength);

        name_Adjective = Enum.ToObject(typeof(Adjective), adjNum).ToString();
        name_Noun = Enum.ToObject(typeof(Noun), nounNum).ToString();
        string name = name_Adjective + " " + name_Noun;
        return name;
    }
}
