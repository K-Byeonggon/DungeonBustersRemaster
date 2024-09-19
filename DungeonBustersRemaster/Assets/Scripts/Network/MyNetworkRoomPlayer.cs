using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkRoomPlayer : NetworkRoomPlayer
{
    [SyncVar(hook = nameof(CharacterIndexChanged))]
    public int characterIndex;

    public int charIndex;

    public override void Start()
    {
        base.Start();
        Debug.Log("Start는 언제임");
        NotifyInitializedNextFrame().Forget();
    }


    public override void OnStartClient()
    {
        Debug.Log("StartClient는 언제임");
        base.OnStartClient();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    public override void OnClientEnterRoom()
    {
        Debug.Log("ClientEnterRoom은 언제임");
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
    
    [Command]
    public void CmdChangeCharacterIndex(int index)
    {
        characterIndex = index;
    }

    #endregion




    #region hook

    private void CharacterIndexChanged(int oldIndex, int newIndex)
    {
        Debug.Log($"{oldIndex} -> {newIndex}");
        UI_Room roomUI = UIManager.Instance.GetActiveUI(UIPrefab.RoomUI).GetComponent<UI_Room>();
        roomUI.UpdatePlayerList();
    }
    #endregion
}
