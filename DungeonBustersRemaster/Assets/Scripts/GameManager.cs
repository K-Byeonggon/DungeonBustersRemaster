using Cysharp.Threading.Tasks;
using System;

public class GameManager : SingletonMono<GameManager>
{
    protected override async void Awake()
    {
        base.Awake();

        if(Instance == this)
        {
            await UniTask.WhenAll
            (
                UIManager.Instance.LoadAllUIPrefabs()
            );

            UIManager.Instance.ShowUI(UIPrefab.TitleUI);
        }
    }
}
