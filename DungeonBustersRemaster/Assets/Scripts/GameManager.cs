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
                UIManager.Instance.LoadAllUIPrefabs(),
                SpriteManager.Instance.LoadAllSprites(),
                MonsterDataManager.Instance.LoadMonsterData(),
                MonsterDataManager.Instance.LoadAllMonsterPrefabs()
            );

            MonsterDataManager.Instance.VerifyLoadedMonsters();
            UIManager.Instance.ShowUI(UIPrefab.LobbyUI);
        }
    }
}
