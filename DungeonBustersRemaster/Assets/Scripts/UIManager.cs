using Cysharp.Threading.Tasks;
using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//UI 프리펩을 만들고 프리펩 이름을 UIPrefab에 추가할 것.
public enum UIPrefab
{
    //임시
    TempUI,
    TempUI2,

    //어디서든
    NotifyUI,

    //타이틀
    TitleUI,

    //Lobby(Offline) Scene
    LobbyUI,
    ClientConnectUI,
    SetPlayerNumUI,
    ClientDisconnectUI,

    //Room(Online) Scene
    RoomUI,
    Content_RoomPlayer

}

public static class UIPrefabExtensions
{
    public static string GetPrefabName(this UIPrefab uiPrefab)
    {
        return uiPrefab.ToString();
    }
}

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, GameObject> uiPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> activeUIs = new Dictionary<string, GameObject>();
    private Dictionary<string, CancellationTokenSource> removeTimers = new Dictionary<string, CancellationTokenSource>();
    private Dictionary<string, Stack<GameObject>> uiPool = new Dictionary<string, Stack<GameObject>>();
    private Dictionary<string, Stack<UIPrefab>> sceneUIs = new Dictionary<string, Stack<UIPrefab>>(); 

    private const float UI_REMOVE_DELAY = 7f;

    public async UniTask LoadAllUIPrefabs()
    {
        var prefabs = Resources.LoadAll<GameObject>("Prefabs/UI");
        foreach (var prefab in prefabs)
        {
            if (prefab != null)
            {
                uiPrefabs[prefab.name] = prefab;
            }
        }
        await UniTask.Yield();
    }

    public void ShowUI(UIPrefab uiPrefab)
    {
        //해당 프리펩 이 uiPrefabs에 있는지 검사
        string uiName = uiPrefab.GetPrefabName();
        if (!uiPrefabs.TryGetValue(uiName, out var prefab))
        {
            Debug.LogError($"UI Prefab '{uiName}' not found!");
            return;
        }

        //UI 인스턴스의 이름을 정해주기
        string instanceName = $"@UI_{uiName}";

        //해당 이름의 인스턴스가 activeUIs에 있는지 검사
        if (!activeUIs.TryGetValue(instanceName, out var uiInstance))   //없으면 넣어줌
        {
            uiInstance = GetOrCreateUIInstance(prefab, instanceName);
            activeUIs[instanceName] = uiInstance;
        }
        else
        {
            if(uiInstance == null)  //있는데 null이면 다시 넣어줌.
            {
                uiInstance = GetOrCreateUIInstance(prefab, instanceName);
                activeUIs[instanceName] = uiInstance;
            }
            else        //그냥 비활성화 상태면 다시 켜줌.
            {
                uiInstance.SetActive(true);
                CancelUITimer(instanceName);
            }
        }

        //Scene 등록
        RegisterSceneUI(uiPrefab);
    }

    private GameObject GetOrCreateUIInstance(GameObject prefab, string instanceName)
    {
        var canvas = GameObject.Find("@UI_Canvas");
        if(canvas == null)
        {
            Debug.LogError("@UI_Canvas not found!");
            return null;
        }

        //Pool에 있었음.
        if(uiPool.TryGetValue(instanceName, out var pool) && pool.Count > 0)
        {
            GameObject uiInstance = pool.Pop();
            uiInstance.transform.SetParent(canvas.transform);
            uiInstance.SetActive(true);
            uiInstance.transform.SetAsLastSibling();    //캔버스의 가장 앞에 위치하도록
            return uiInstance;
        }
        else //Pool에 없었음.
        {
            GameObject uiInstance = UnityEngine.Object.Instantiate(prefab, canvas.transform);
            uiInstance.name = instanceName;
            return uiInstance;
        }
    }

    //UI를 숨기고 Pool에 넣는다. (자주 사용되는 UI에 적용)
    public void HideUIWithPooling(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            uiInstance.SetActive(false);
            activeUIs.Remove(instanceName);

            if (!uiPool.ContainsKey(instanceName))
            {
                uiPool[instanceName] = new Stack<GameObject>();
            }
            uiPool[instanceName].Push(uiInstance);
        }

    }

    //UI를 숨기고 Timer를 작동해서 시간이 지나면 파괴한다. (드물게 사용되는 UI에 적용)
    public void HideUIWithTimer(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";
        
        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            uiInstance.SetActive(false);
            StartRemoveUITimer(instanceName);
        }
    }

    //그냥 UI를 파괴한다. (씬전환시 필요없거나 초기화되어야하는 UI들에 사용)
    public void DestroyUI(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            UnityEngine.Object.Destroy(uiInstance);
            activeUIs.Remove(instanceName);
        }
    }

    private void StartRemoveUITimer(string instanceName)
    {
        CancelUITimer(instanceName);

        var cts = new CancellationTokenSource();
        removeTimers[instanceName] = cts;

        ExpiredUI(instanceName, UI_REMOVE_DELAY, cts.Token).Forget();
    }
    
    public GameObject GetActiveUI(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";

        if(activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            return uiInstance;
        }
        return null;
    }


    private async UniTask ExpiredUI(string instanceName, float delay, CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);

            if(activeUIs.TryGetValue(instanceName, out var uiInstance) && !uiInstance.activeSelf)
            {
                UnityEngine.Object.Destroy(uiInstance);
                activeUIs.Remove(instanceName);
                removeTimers.Remove(instanceName);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"UI '{instanceName}' removal canceled.");
        }
        catch (MissingReferenceException)
        {
            Debug.Log($"UI '{instanceName}' has already been destroyed.");
            activeUIs.Remove(instanceName);
            removeTimers.Remove(instanceName);
        }
    }

    private void CancelUITimer(string instanceName)
    {
        if(removeTimers.TryGetValue(instanceName, out var cts))
        {
            cts.Cancel();
            removeTimers.Remove(instanceName);
        }
    }

    //목록에 넣는 Content 같은 UI 가져오기
    public GameObject JustGetUIPrefab(UIPrefab uiPrefab)
    {
        string uiName = uiPrefab.GetPrefabName();
        if (!uiPrefabs.TryGetValue(uiName, out GameObject prefab))
        {
            Debug.LogError($"UI Prefab '{uiName}' not found!");
            return null;
        }

        return prefab;
    }

    //Scene별로 UI 등록하기. ShowUI할때 SceneName으로 정리한다.
    private void RegisterSceneUI(UIPrefab uiPrefab)
    {
        string sceneName = NetworkManager.networkSceneName;
        string uiName = uiPrefab.GetPrefabName();
        string instanceName = $"@UI_{uiName}";

        if (activeUIs.TryGetValue(instanceName, out var uiInstance))
        {
            if (!sceneUIs.ContainsKey(sceneName))
            {
                sceneUIs[sceneName] = new Stack<UIPrefab>();
            }
            sceneUIs[sceneName].Push(uiPrefab);
        }
    }

    //Scene에 등록된 UI전부 제거하기
    public void DestroySceneUI(string sceneName)
    {
        if (sceneUIs.ContainsKey(sceneName))
        {
            foreach (var uiPrefab in sceneUIs[sceneName])
            {
                DestroyUI(uiPrefab);
            }
        }
    }
}
