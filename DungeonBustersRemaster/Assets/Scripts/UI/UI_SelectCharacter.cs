using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectCharacter : MonoBehaviour
{
    [SerializeField] Button Btn_Background;
    [SerializeField] List<Button> Btn_Character;

    private void OnEnable()
    {
        Btn_Background.onClick.AddListener(OnClick_Background);

        for (int i = 0; i < Btn_Character.Count; i++)
        {
            int index = i;
            Btn_Character[index].onClick.AddListener(() => OnClick_Character(index));
        }
    }

    private void OnDisable()
    {
        Btn_Background.onClick.RemoveListener(OnClick_Background);

        for (int i = 0; i < Btn_Character.Count; i++)
        {
            Btn_Character[i].onClick.RemoveAllListeners();
        }
    }

    private void OnClick_Background()
    {
        UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
    }

    private void OnClick_Character(int index)
    {
        Debug.Log($"{index}번 캐릭터 선택");
        //여기서 MyNetworkRoomPlayer의 characterIndex 변경. 
        NetworkRoomPlayer roomPlayer = MyNetworkRoomManager.Instance.GetLocalRoomPlayer();
        MyNetworkRoomPlayer myRoomPlayer = roomPlayer.GetComponent<MyNetworkRoomPlayer>();
        myRoomPlayer.CmdChangeCharacterIndex(index);

        UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
    }
}
