using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public int characterIndex;
    public string nickname;

    public PlayerData(int characterIndex, string nickname)
    {
        this.characterIndex = characterIndex;
        this.nickname = nickname;
    }
}

public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private Dictionary<uint, PlayerData> playerData = new Dictionary<uint, PlayerData>();

    public void SetPlayerData(uint netId, int characterIndex, string nickname)
    {
        playerData[netId] = new PlayerData(characterIndex, nickname);
    }

    public int GetCharacterIndex(uint netId)
    {
        if (!playerData.ContainsKey(netId))
        {
            SetPlayerData(netId, -1, "Unknown");
        }

        return playerData[netId].characterIndex;
    }

    public string GetNickname(uint netId)
    {
        if (!playerData.ContainsKey(netId))
        {
            SetPlayerData(netId, -1, "Unknown");
        }

        return playerData[netId].nickname;
    }

    public void RemovePlayerData(uint netId)
    {
        if (playerData.ContainsKey(netId))
        {
            playerData.Remove(netId);
        }
    }

    public void ClearAllData()
    {
        playerData.Clear();
    }
}
