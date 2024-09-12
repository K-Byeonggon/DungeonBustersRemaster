using UnityEngine;
using UnityEngine.UI;

public class UI_Room : MonoBehaviour
{
    [SerializeField] GameObject Layout_Players;
    [SerializeField] Button Btn_StartGame;
    [SerializeField] Button Btn_ExitRoom;

    private void OnEnable()
    {
        Btn_StartGame.onClick.AddListener(OnClick_StartGame);
        Btn_ExitRoom.onClick.AddListener(OnClick_ExitRoom);
    }

    private void OnDisable()
    {
        Btn_StartGame.onClick.RemoveListener(OnClick_StartGame);
        Btn_ExitRoom.onClick.RemoveListener(OnClick_ExitRoom);
    }

    private void OnClick_StartGame()
    {

    }

    private void OnClick_ExitRoom()
    {

    }
}
