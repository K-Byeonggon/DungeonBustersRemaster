using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_OpenCard : MonoBehaviour
{
    [SerializeField] Transform Layout_Players;
    [SerializeField] TextMeshProUGUI Text_Formula;
    [SerializeField] TextMeshProUGUI Text_Result;

    [Header("OpenPlayerPrefab")]
    [SerializeField] GameObject Prefab_PanelOpenPlayer;



    //아마 GameLogicManager에서 전투결과 모두 계산되면 ClientRpc로 모든 클라에서 실행될듯.
    public void SetOpenPlayer()
    {
        foreach(Transform child in Layout_Players)
        {
            Destroy(child.gameObject);
        }
        
        //아마 spawned 순서로 플레이어 데이터 정렬되니까 그냥 Get하면 PlayerData랑 PlayerGameData랑 순서대로 잘 받지 않을까?
        foreach (var kvp in NetworkClient.spawned)
        {
            if (kvp.Value.TryGetComponent(out MyPlayer playerData)
                && kvp.Value.TryGetComponent(out MyPlayerGameData playerGameData))
            {
                GameObject gObj = Instantiate(Prefab_PanelOpenPlayer, Layout_Players);
                Panel_OpenPlayer openPlayer = gObj.GetComponent<Panel_OpenPlayer>();

                openPlayer.SetPlayerName(playerData.Nickname);
                openPlayer.SetCharacterIcon(playerData.CharacterIndex);
                openPlayer.SetSelectedCard(playerGameData.SubmittedCardNum, playerData.PlayerColor);
                openPlayer.SetAttackSuccess(playerGameData.IsAttackSuccess);
            }
        }
    }

    public void PlayCardOpenAnimation()
    {
        //카드가 순차적으로 펼쳐지는 그런거 넣어보고 싶음.
        
    }

    public void PlayAttackFailAnimation()
    {
        //중복된 카드에 X표시가 생기는 애니메이션 넣어보고 싶음.
    }
}
