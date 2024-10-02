using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GetReward : MonoBehaviour
{
    [SerializeField] Transform Layout_Reward;
    [SerializeField] Button Btn_Confirm;

    private void OnEnable()
    {
        Btn_Confirm.onClick.AddListener(OnClick_Confirm);
    }

    private void OnDisable()
    {
        Btn_Confirm.onClick.RemoveListener(OnClick_Confirm);
    }

    private void OnClick_Confirm()
    {
        UIManager.Instance.HideUIWithPooling(UIPrefab.GetRewardUI);
        //GameLogicManger에 신호보내기
    }

    private void SetLayoutReward(List<int> reward)
    {
        GameObject prefabGem = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_Gem);
        for(int index = 0;  index < reward.Count; index++)
        {
            for(int i = 0; i < reward[index];  i++)
            {
                GameObject gObj = Instantiate(prefabGem, Layout_Reward);
                Content_Gem gem = gObj.GetComponent<Content_Gem>();
                gem.SetGemImg((GemColor)index);
            }
        }
    }


    public static void Show(List<int> reward)
    {
        UIManager.Instance.ShowUI(UIPrefab.GetRewardUI);
        UI_GetReward getRewardUI = UIManager.Instance.GetActiveUI(UIPrefab.GetRewardUI).GetComponent<UI_GetReward>();
        getRewardUI.SetLayoutReward(reward);
    }

}
