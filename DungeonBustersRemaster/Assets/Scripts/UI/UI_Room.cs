using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Room : MonoBehaviour
{
    [SerializeField] Transform Layout_Players;
    [SerializeField] Button Btn_Ready;
    [SerializeField] TextMeshProUGUI Text_Ready;
    [SerializeField] Button Btn_ExitRoom;
    [SerializeField] Button Btn_SelectCharacter;
    [SerializeField] Button Btn_ChangeName;

    private NetworkRoomPlayer localRoomPlayer;

    public Transform Layout_RoomPlayers
    {
        get { return Layout_Players; }
    }

    private GameObject contentRoomPlayerPrefab;

    private void OnEnable()
    {
        contentRoomPlayerPrefab = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_RoomPlayer);

        MyNetworkRoomManager.Instance.OnRoomPlayersUpdated += UpdatePlayerList;

        Btn_Ready.onClick.AddListener(OnClick_Ready);
        Btn_ExitRoom.onClick.AddListener(OnClick_ExitRoom);
        Btn_SelectCharacter.onClick.AddListener(OnClick_SelectCharacter);
        Btn_ChangeName.onClick.AddListener(OnClick_ChangeName);
    }

    private void OnDisable()
    {
        MyNetworkRoomManager.Instance.OnRoomPlayersUpdated -= UpdatePlayerList;
     
        Btn_Ready.onClick.RemoveListener(OnClick_Ready);
        Btn_ExitRoom.onClick.RemoveListener(OnClick_ExitRoom);
        Btn_SelectCharacter.onClick.RemoveListener(OnClick_SelectCharacter);
        Btn_ChangeName.onClick.RemoveListener(OnClick_ChangeName);
    }




    private void OnClick_Ready()
    {
        if(localRoomPlayer == null)
        {
            localRoomPlayer = NetworkClient.connection?.identity?.GetComponent<NetworkRoomPlayer>();
        }

        if(localRoomPlayer != null)
        {
            Text_Ready.text = localRoomPlayer.readyToBegin ? "준비" : "준비해제";
            localRoomPlayer.CmdChangeReadyState(!localRoomPlayer.readyToBegin);
        }
    }

    private void OnClick_ExitRoom()
    {
        if (NetworkServer.active && NetworkClient.active)
        {
            MyNetworkRoomManager.Instance.StopHost();
        }
        else if (NetworkClient.active)
        {
            MyNetworkRoomManager.Instance.StopClient();
        }

        SceneManager.LoadScene(MyNetworkRoomManager.Instance.offlineScene);

        UIManager.Instance.HideUIWithTimer(UIPrefab.RoomUI);
        UIManager.Instance.ShowUI(UIPrefab.LobbyUI);
    }

    private void OnClick_SelectCharacter()
    {
        UIManager.Instance.ShowUI(UIPrefab.SelectCharacterUI);
    }

    private void OnClick_ChangeName()
    {
        UIManager.Instance.ShowUI(UIPrefab.ChangeNameUI);
    }

    public void UpdatePlayerList()
    {
        ClearPlayerList();
        foreach(NetworkRoomPlayer player in MyNetworkRoomManager.Instance.roomSlots)
        {
            MyNetworkRoomPlayer myPlayer = player.GetComponent<MyNetworkRoomPlayer>();


            GameObject roomPlayer = Instantiate(contentRoomPlayerPrefab, Layout_Players);
            UI_RoomPlayer roomPlayerUI = roomPlayer.GetComponent<UI_RoomPlayer>();
            string playerName = string.IsNullOrWhiteSpace(myPlayer.nickname) ? $"Player{player.index + 1}" : myPlayer.nickname;
            roomPlayerUI.SetPlayerInfo(playerName, player.readyToBegin, myPlayer.characterIndex);
        }
    }

    private void ClearPlayerList()
    {
        foreach (Transform child in Layout_Players)
        {
            Destroy(child.gameObject);
        }
    }
}
