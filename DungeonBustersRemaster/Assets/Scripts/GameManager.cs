using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        TestUIManager();
    }

    private async void TestUIManager()
    {

        await UniTask.WhenAll
        (
            UIManager.Instance.LoadAllUIPrefabs()
        );

        UIManager.Instance.ShowUI(UIPrefab.TempUI);
        UIManager.Instance.HideUIWithTimer(UIPrefab.TempUI);

        UIManager.Instance.ShowUI(UIPrefab.TempUI2);
        UIManager.Instance.HideUIWithPooling(UIPrefab.TempUI2);

        await UniTask.Delay(TimeSpan.FromSeconds(10));
        UIManager.Instance.ShowUI(UIPrefab.TempUI);


        await UniTask.Delay(TimeSpan.FromSeconds(2));
        UIManager.Instance.ShowUI(UIPrefab.TempUI2);
    }


}
