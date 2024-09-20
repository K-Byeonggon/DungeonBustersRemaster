using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    //Room에서 고른 캐릭터의 index가 netId를 키로 저장된다. 
    //나중에는 이름도 저장해야할 수 있어서, index대신 PlayerData 클래스를 Value로 저장할 수도 있다.
    private Dictionary<uint, int> playerCharacterIndex = new Dictionary<uint, int>();

    public void SetCharacterIndex(uint netId, int characterIndex)
    {
        playerCharacterIndex[netId] = characterIndex;
    }

    public int GetCharacterIndex(uint netId)
    {
        if (!playerCharacterIndex.ContainsKey(netId))
        {
            SetCharacterIndex(netId, 0);
        }

        return playerCharacterIndex.ContainsKey(netId) ? playerCharacterIndex[netId] : -1;
    }

    public void RemovePlayerData(uint netId)
    {
        if (playerCharacterIndex.ContainsKey(netId))
        {
            playerCharacterIndex.Remove(netId);
        }
    }

    public void ClearAllData()
    {
        playerCharacterIndex.Clear();
    }
}
