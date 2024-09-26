using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Dungeon;
    [SerializeField] TextMeshProUGUI Text_Stage;
    [SerializeField] Button Btn_Config;

    private void OnEnable()
    {
        Btn_Config.onClick.AddListener(OnClick_Config);
    }

    private void OnDisable()
    {
        Btn_Config.onClick.RemoveListener(OnClick_Config);
    }

    private void OnClick_Config()
    {

    }

    private void SetDungeonInfo(int currentDungeon)
    {
        Text_Dungeon.text = $"Dungeon {currentDungeon}";
    }

    private void SetStageInfo(int currentStage)
    {
        Text_Stage.text = $"Stage {currentStage}";
    }

    public static void UpdateDungeonInfo(int currentDungeon)
    {
        UI_StageInfo stageInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.StageInfoUI).GetComponent<UI_StageInfo>();
        stageInfoUI.SetDungeonInfo(currentDungeon);
    }

    public static void UpdateStageInfo(int currentStage)
    {
        UI_StageInfo stageInfoUI = UIManager.Instance.GetActiveUI(UIPrefab.StageInfoUI).GetComponent<UI_StageInfo>();
        stageInfoUI.SetStageInfo(currentStage);
    }
}
