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

    public override void OnRoomClientSceneChanged()
    {
        if (Utils.IsSceneActive(GameplayScene))
        {
            UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.ChangeNameUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);
        }
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
    }




    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        GameObject newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
        MyNetworkRoomPlayer myRoomPlayer = newRoomGameObject.GetComponent<MyNetworkRoomPlayer>();
        //Lobby Scene에서 캐릭터를 고른다면, 여기에서 MyNetworkManager 등에 저장된 캐릭터index를 넣어줄것

        return newRoomGameObject;
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
            UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);
            UIManager.Instance.ShowUI(UIPrefab.LobbyUI);
            UI_Notify.Show("호스트가 연결을 끊어 종료되었습니다.");
        }
    }


    //GameScene에 GamePlayer 생성
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        MethodName.DebugLog();

        Transform startPos = GetStartPosition();
        GameObject gamePlayer = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        MyPlayer myGamePlayer = gamePlayer.GetComponent<MyPlayer>();

        //서버에서 바로 characterIndex 설정
        int characterIndex = PlayerDataManager.Instance.GetCharacterIndex(conn.identity.netId);
        myGamePlayer.characterIndex = characterIndex;

        return gamePlayer;
    }

    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
        Debug.Log("이건 불리는지 확인");
    }




    public NetworkRoomPlayer GetLocalRoomPlayer()
    {
        // NetworkClient.localPlayer는 현재 클라이언트가 소유한 NetworkIdentity입니다.
        if (NetworkClient.localPlayer != null)
        {
            // roomSlots에서 localPlayer와 같은 NetworkIdentity를 가진 RoomPlayer 찾기
            foreach (NetworkRoomPlayer roomPlayer in roomSlots)
            {
                if (roomPlayer != null && roomPlayer.netIdentity == NetworkClient.localPlayer)
                {
                    return roomPlayer; // 로컬 플레이어 반환
                }
            }
        }

        return null; // 찾지 못한 경우 null 반환
    }



}
