using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Result : MonoBehaviour
{
    [SerializeField] Image Img_PlayerIcon;
    [SerializeField] TextMeshProUGUI Text_PlayerName;
    [SerializeField] List<TextMeshProUGUI> Text_GemCounts;
    [SerializeField] TextMeshProUGUI Text_GemSetCount;
    [SerializeField] TextMeshProUGUI Text_TotalPoint;

    private uint panelNetId;
    private MyPlayer playerData;
    private MyPlayerGameData playerGameData;

    public uint PanelNetId
    {
        get { return panelNetId;}
        set
        {
            panelNetId = value;
            RegistPlayerData(panelNetId);
        }
    }

    public void UpdatePlayerName()
    {
        Text_PlayerName.text = playerData.Nickname;
    }

    public void UpdatePlayerIcon()
    {
        Img_PlayerIcon.sprite = SpriteManager.Instance.GetCharacterIconSprite(playerData.CharacterIndex);
    }

    public void UpdateGemCounts()
    {
        for(int i = 0; i < Text_GemCounts.Count; i++)
        {
            Text_GemCounts[i].text = $"+{playerGameData.Gems[i]}";
        }
    }

    public void UpdateGemSetCount()
    {
        int minGemCount = playerGameData.Gems.Min();
        Text_GemSetCount.text = $"+{minGemCount} ×3";
    }

    public void UpdateTotalPoint()
    {
        int totalPoint = 0;

        foreach(int count in playerGameData.Gems)
        {
            totalPoint += count;
        }

        totalPoint += playerGameData.Gems.Min() * 3;

        Text_TotalPoint.text = $"점수: {totalPoint}";
    }


    private void RegistPlayerData(uint netId)
    {
        if(NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity identity))
        {
            playerData = identity.GetComponent<MyPlayer>();
            playerGameData = identity.GetComponent<MyPlayerGameData>();
        }
    }
}
