using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DungeonClear : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_DungeonClear;

    private void UpdateTextDungeonClear()
    {
        int floor = GameLogicManager.Instance.CurrentDungeon;
        Text_DungeonClear.text = $"던전 {floor}층 돌파!";
    }


    public static void Show()
    {
        UIManager.Instance.ShowUI(UIPrefab.DungeonClearUI);
        UI_DungeonClear dungeonClearUI = UIManager.Instance.GetActiveUI(UIPrefab.DungeonClearUI).GetComponent<UI_DungeonClear>();
        dungeonClearUI.UpdateTextDungeonClear();
    }
}
