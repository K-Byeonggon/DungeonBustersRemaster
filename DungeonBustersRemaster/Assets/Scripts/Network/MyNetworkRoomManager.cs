using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomManager : NetworkRoomManager
{
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

    public event Action OnPlayerAdded;

    public event Action OnPlayerRemoved;

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        base.OnRoomServerSceneChanged(sceneName);
    }

    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnRoomServerAddPlayer(conn);
    }

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();

    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        OnPlayerAdded?.Invoke();

    }
    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);

        OnPlayerRemoved?.Invoke();
    
    
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

        UIManager.Instance.ShowUI(UIPrefab.ClientDisconnectUI);
    }
}
