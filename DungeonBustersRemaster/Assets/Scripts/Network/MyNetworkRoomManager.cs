using Mirror;
using Mirror.Examples.MultipleMatch;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyNetworkRoomManager : NetworkRoomManager
{
    public event Action OnRoomPlayersUpdated;


    #region Singleton
    public static MyNetworkRoomManager Instance { get; private set; }

    public override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();
    }
    #endregion



    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        MethodName.DebugLog();
        //이거만 제대로 동작하는듯?
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        base.OnServerSceneChanged(newSceneName);
        MethodName.DebugLog();
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
        MethodName.DebugLog();

    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }






    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
        MethodName.DebugLog();
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MethodName.DebugLog();
        //MyNetworkRoomPlayer에서 Start가 완료될 때까지 대기해야함.
    }

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
    }

    public override void OnRoomServerPlayersNotReady()
    {
        base.OnRoomServerPlayersNotReady();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        MethodName.DebugLog();

        base.OnServerDisconnect(conn);

    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        MethodName.DebugLog();

        base.OnRoomServerDisconnect(conn);
        OnRoomPlayersUpdated?.Invoke();
    }

    public void NotifyPlayerInitialized()
    {
        OnRoomPlayersUpdated?.Invoke();
    }


    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("클라이언트가 서버와 연결됨.");

        UIManager.Instance.HideUIWithTimer(UIPrefab.SetPlayerNumUI);
        UIManager.Instance.HideUIWithTimer(UIPrefab.ClientConnectUI);
        UIManager.Instance.HideUIWithTimer(UIPrefab.LobbyUI);
        UIManager.Instance.ShowUI(UIPrefab.RoomUI);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("클라이언트가 서버와의 연결에 실패함.");

        if (Utils.IsSceneActive(offlineScene))
        {
            UIManager.Instance.ShowUI(UIPrefab.ClientDisconnectUI);
        }

        if (Utils.IsSceneActive(onlineScene))
        {
            UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);
            UIManager.Instance.ShowUI(UIPrefab.LobbyUI);
            UI_Notify.Show("호스트가 연결을 끊어 종료되었습니다.");
        }
    }


    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        Debug.Log("이건 불리는지 확인");
    }

    public override void OnRoomClientSceneChanged()
    {
        base.OnRoomClientSceneChanged();
        Debug.Log("이거 불리는지 확인");
    }
}
