using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UI_Room : MonoBehaviour
{
    [SerializeField] Transform Layout_Players;
    [SerializeField] Button Btn_Ready;
    [SerializeField] Button Btn_ExitRoom;

    private GameObject contentRoomPlayerPrefab;

    private void OnEnable()
    {


        MyNetworkRoomManager.Instance.OnPlayerAdded += UpdatePlayerList;
        MyNetworkRoomManager.Instance.OnPlayerRemoved += UpdatePlayerList;

        contentRoomPlayerPrefab = UIManager.Instance.JustGetUIPrefab(UIPrefab.Content_RoomPlayer);

        Btn_Ready.onClick.AddListener(OnClick_Ready);
        Btn_ExitRoom.onClick.AddListener(OnClick_ExitRoom);
    }

    private void OnDisable()
    {
        MyNetworkRoomManager.Instance.OnPlayerAdded -= UpdatePlayerList;
        MyNetworkRoomManager.Instance.OnPlayerRemoved -= UpdatePlayerList;

        Btn_Ready.onClick.RemoveListener(OnClick_Ready);
        Btn_ExitRoom.onClick.RemoveListener(OnClick_ExitRoom);
    }

    #region roomManager


    private void UpdatePlayerList()
    {
        ClearPlayerList();

        foreach(var conn in NetworkServer.connections.Values)
        {
            GameObject playerUI = Instantiate(contentRoomPlayerPrefab, Layout_Players);
        }
    }

    private void ClearPlayerList()
    {
        foreach (Transform child in Layout_Players)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion



    private void OnClick_Ready()
    {

    }

    private void OnClick_ExitRoom()
    {

    }
}
