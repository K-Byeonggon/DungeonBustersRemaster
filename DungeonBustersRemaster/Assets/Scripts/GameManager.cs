using Cysharp.Threading.Tasks;
using System;

public class GameManager : SingletonMono<GameManager>
{
    protected override async void Awake()
    {
        base.Awake();

        await UniTask.WhenAll
        (
            UIManager.Instance.LoadAllUIPrefabs()
        );

        UIManager.Instance.ShowUI(UIPrefab.TitleUI);
    }

    private async void TestUIManager()
    {
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
