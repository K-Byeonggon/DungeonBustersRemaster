using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Panel_Reward : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_RewardNum;
    [SerializeField] Transform Layout_Gems;

    
    public void UpdateRewardInfo(List<int> reward)
    {
        GameObject gemPrefab = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Gem);

        foreach(Transform child in Layout_Gems)
        {
            Destroy(child.gameObject);
        }

        for(int color = 0; color < reward.Count; color++)
        {
            for(int count = 0; count < reward[color]; count++)
            {
                GameObject gObj = Instantiate(gemPrefab, Layout_Gems);
                Content_Gem gem = gObj.GetComponent<Content_Gem>();
                gem.SetGemImg((GemColor)color);
            }
        }

    }
}
