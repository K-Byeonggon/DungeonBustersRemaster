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
    public event Action<int> OnAllGamePlayerLoaded;

    //GamePlayScene에 들어간 플레이수 추적(PlayerColor정할 때 쓰임)
    private int gamePlayerCount = 0;
    
    public int GamePlayerCount => gamePlayerCount;

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



    //Server
    public override void OnServerChangeScene(string newSceneName)
    {
        MethodName.DebugLog();
        base.OnServerChangeScene(newSceneName);
        OnChangeScene(newSceneName);
    }

    //Server
    public override void OnRoomServerSceneChanged(string sceneName)
    {
        MethodName.DebugLog();
        base.OnRoomServerSceneChanged(sceneName);
    }

    //Server
    public override void OnServerSceneChanged(string newSceneName)
    {
        MethodName.DebugLog();
        base.OnServerSceneChanged(newSceneName);
    }


    //Server, Client
    public override void OnClientConnect()
    {
        MethodName.DebugLog();
        base.OnClientConnect();
        Debug.Log("클라이언트가 서버와 연결됨.");
    }

    //Server, Client
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        MethodName.DebugLog();
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        OnChangeScene(newSceneName);
    }

    // Client
    public override void OnClientSceneChanged()
    {
        MethodName.DebugLog();

        base.OnClientSceneChanged();


    }

    // Client
    public override void OnRoomClientSceneChanged()
    {
        MethodName.DebugLog();
    }

    //Server
    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
    {
        MethodName.DebugLog();
        GameObject newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
        MyNetworkRoomPlayer myRoomPlayer = newRoomGameObject.GetComponent<MyNetworkRoomPlayer>();
        //Lobby Scene에서 캐릭터를 고른다면, 여기에서 MyNetworkManager 등에 저장된 캐릭터index를 넣어줄것

        return newRoomGameObject;
    }

    //Server
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        MethodName.DebugLog();
        //MyNetworkRoomPlayer에서 Start가 완료될 때까지 대기해야함.
    }

    //MyNetworkRoomPlayer: OnStartClient

    //MyNetworkRoomPlayer: OnClientEnterRoom

    public void NotifyPlayerInitialized()
    {
        MethodName.DebugLog();
        OnRoomPlayersUpdated?.Invoke();
    }


    // ######################################## GamePlay Scene ########################################

    //Server
    public override void OnRoomServerPlayersReady()
    {
        MethodName.DebugLog();
        base.OnRoomServerPlayersReady();
    }

    //Server: OnServerChangeScene

    //Server, Client: OnClientChangeScene

    //Server: OnRoomServerSceneChanged

    //Server: OnServerSceneChanged

    //Client : OnClientSceneChanged


    //Server, Client : OnRoomClientSceneChanged




    //Server
    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
    {
        MethodName.DebugLog();

        gamePlayerCount++;

        Transform startPos = GetStartPosition();
        GameObject gamePlayer = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        MyPlayer myGamePlayer = gamePlayer.GetComponent<MyPlayer>();
        MyPlayerGameData myGameData = gamePlayer.GetComponent<MyPlayerGameData>();

        //서버에서 바로 playerInfo 설정(클라이언트는 다른 설정 없이 이 설정을 동기화받음)
        InitializeGamePlayer(conn, myGamePlayer);
        InitializePlayerGameData(conn, myGameData);

        return gamePlayer;
    }

    [Server]
    private void InitializeGamePlayer(NetworkConnectionToClient conn, MyPlayer myGamePlayer)
    {
        MethodName.DebugLog();
        myGamePlayer.InitializeCharacterIndex(conn);
        myGamePlayer.InitializeNickname(conn);
        myGamePlayer.InitializePlayerColor(gamePlayerCount);
    }

    [Server]
    private void InitializePlayerGameData(NetworkConnectionToClient conn, MyPlayerGameData myGameData)
    {
        MethodName.DebugLog();
        myGameData.InitializeHands();
        myGameData.InitializeGems();
        myGameData.InitializeUsedCards();
    }


    //Server. GamePlayScene에 플레이어가 생성완료되면 불린다. 여기서 플레이어를 체크해서 게임을 시작하자.
    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        MethodName.DebugLog();
        Debug.Log(roomSlots.Count);
        if (gamePlayerCount == roomSlots.Count)
        {
            OnAllGamePlayerLoaded?.Invoke(gamePlayerCount);
        }

        return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
    }



    private void OnChangeScene(string sceneName)
    {
        //씬이 미리 바뀌기 전에 씬 이름으로 UI활성화하는 느낌.

        //Lobby Scene
        if (sceneName == offlineScene)
        {
            //LobbySceneUI 활성화
            UIManager.Instance.ShowUI(UIPrefab.LobbyUI);

            //RoomSceneUI 숨기기
            UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.ChangeNameUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);

            //GameSceneUI 숨기기
            UIManager.Instance.HideUIWithTimer(UIPrefab.StageInfoUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.MonsterInfoUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.BonusGemsUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.TimerUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.CardPanelUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.PlayerInfoUI);
        }

        //Room Scene
        if (sceneName == onlineScene)
        {
            UIManager.Instance.ShowUI(UIPrefab.RoomUI);

            //LobbySceneUI 숨기기
            UIManager.Instance.HideUIWithTimer(UIPrefab.LobbyUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.ClientConnectUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.SetPlayerNumUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.ClientDisconnectUI);
        }

        //GamePlayer Scene
        if (sceneName == GameplayScene)
        {
            //RoomSceneUI 숨기기
            UIManager.Instance.HideUIWithTimer(UIPrefab.SelectCharacterUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.ChangeNameUI);
            UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);

            //GameSceneUI 활성화
            UIManager.Instance.ShowUI(UIPrefab.StageInfoUI);
            UIManager.Instance.ShowUI(UIPrefab.MonsterInfoUI);
            UIManager.Instance.ShowUI(UIPrefab.BonusGemsUI);
            UIManager.Instance.ShowUI(UIPrefab.TimerUI);
            UIManager.Instance.ShowUI(UIPrefab.CardPanelUI);
            UIManager.Instance.ShowUI(UIPrefab.PlayerInfoUI);
        }
    }








    public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
    {
        MethodName.DebugLog();
        base.OnRoomServerAddPlayer(conn);
    }



    public override void OnRoomServerPlayersNotReady()
    {
        MethodName.DebugLog();
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



    public override void OnClientDisconnect()
    {
        MethodName.DebugLog();
        base.OnClientDisconnect();
        Debug.Log("클라이언트가 서버와의 연결에 실패함.");

        if (Utils.IsSceneActive(offlineScene))
        {
            UIManager.Instance.ShowUI(UIPrefab.ClientDisconnectUI);
        }

        if (Utils.IsSceneActive(onlineScene))
        {
            //offlineScene으로
            UnityEngine.SceneManagement.SceneManager.LoadScene(offlineScene);

            UI_Notify.Show("호스트가 연결을 끊어 종료되었습니다.");
        }

        if (Utils.IsSceneActive(GameplayScene))
        {
            //UIManager.Instance.HideUIWithTimer(UIPrefab.GameSceneUI);
        }
    }










    #region tools

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

    public List<MyPlayerGameData> GetAllPlayerGameDatas()
    {
        List<MyPlayerGameData> playerGameDataList = new List<MyPlayerGameData>();
        foreach(var kvp in NetworkClient.spawned)
        {
            NetworkIdentity identity = kvp.Value;

            MyPlayerGameData playerGameData = identity.GetComponent<MyPlayerGameData>();
            if (playerGameData != null)
            {
                playerGameDataList.Add(playerGameData);
            }
        }
        return playerGameDataList;
    }

    #endregion
}
