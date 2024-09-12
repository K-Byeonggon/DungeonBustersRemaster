using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    [SerializeField] Button Btn_StartGame;
    [SerializeField] Button Btn_QuitGame;

    private void OnEnable()
    {
        Btn_StartGame.onClick.AddListener(OnClick_StartGame);
        Btn_QuitGame.onClick.AddListener(OnClick_QuitGame);
    }

    private void OnDisable()
    {
        Btn_StartGame.onClick.RemoveListener(OnClick_StartGame);
        Btn_QuitGame.onClick.RemoveListener(OnClick_QuitGame);
    }

    private void OnClick_StartGame()
    {
        Debug.Log("OnClick_StartGame");
        UIManager.Instance.HideUIWithTimer(UIPrefab.TitleUI);
        UIManager.Instance.ShowUI(UIPrefab.LobbyUI);
    }

    private void OnClick_QuitGame()
    {
        Debug.Log("OnClick_QuitGame");

        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
