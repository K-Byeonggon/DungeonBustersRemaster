using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(CharacterIndexChanged))]
    public int characterIndex = -1;

    [SyncVar(hook = nameof(NicknameChanged))]
    public string nickname;

    public override void Start()
    {
        base.Start();

        string defalutName = $"Player{index + 1}";
        CmdChangeCharacterIndex(0);
        CmdChangeNickName(defalutName);

        NotifyInitializedNextFrame().Forget();
    }


    public override void OnStartClient()
    {
        MethodName.DebugLog();
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        uint netId = GetComponent<NetworkIdentity>().netId;
        PlayerDataManager.Instance.RemovePlayerData(netId);
    }

    public override void OnClientEnterRoom()
    {
        MethodName.DebugLog();
        base.OnClientEnterRoom();
    }

    private async UniTaskVoid NotifyInitializedNextFrame()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);

        MyNetworkRoomManager.Instance.NotifyPlayerInitialized();
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        UI_Room roomUI = UIManager.Instance.GetActiveUI(UIPrefab.RoomUI).GetComponent<UI_Room>();
        roomUI.UpdatePlayerList();
    }




    #region commands

    [Command(requiresAuthority = false)]
    public void CmdChangeCharacterIndex(int index)
    {
        characterIndex = index;
    }


    [Command(requiresAuthority = false)]
    public void CmdChangeNickName(string name)
    {
        nickname = name;
    }

    #endregion




    #region hook

    private void CharacterIndexChanged(int oldIndex, int newIndex)
    {
        uint netId = GetComponent<NetworkIdentity>().netId;
        PlayerDataManager.Instance.SetPlayerData(netId, newIndex, nickname);

        UI_Room roomUI = UIManager.Instance.GetActiveUI(UIPrefab.RoomUI).GetComponent<UI_Room>();
        roomUI.UpdatePlayerList();
    }

    private void NicknameChanged(string oldName, string  newName)
    {
        uint netId = GetComponent<NetworkIdentity>().netId;
        PlayerDataManager.Instance.SetPlayerData(netId, characterIndex, newName);

        UI_Room roomUI = UIManager.Instance.GetActiveUI(UIPrefab.RoomUI).GetComponent<UI_Room>();
        roomUI.UpdatePlayerList();
    }
    #endregion
}
