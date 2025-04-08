using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoadingScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject over100PlayersPanel;
    [SerializeField] public Button quitGameButton;
    [SerializeField] private GameObject gameLoadingScenePanel;
    [SerializeField] private TMP_Text gameLoadingText;
    public static GameLoadingScene Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start() {
        StartCoroutine(LoadLobby());
        StartCoroutine(WaitingTextRoutine());
    }
    
    IEnumerator WaitingTextRoutine()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            gameLoadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator LoadLobby() {
        // 네트워크 연결 시도
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
        }

        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;
    }

    public void CheckCountOfPlayer()
    {
        if (PhotonNetwork.CountOfPlayers > 100) {
            ShowOverCapacityUI();
        } else {
            if (gameLoadingScenePanel != null)
            {
                gameLoadingScenePanel.SetActive(false);
            }
            if (SceneManager.GetActiveScene().name == "LoadingScene")
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }

    private void ShowOverCapacityUI()
    {
        if (over100PlayersPanel != null)
        {
            over100PlayersPanel.SetActive(true);
        }
    }
    
    public void QuitGame()
    {
        over100PlayersPanel.SetActive(false);
        gameLoadingScenePanel.SetActive(false);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#endif
    }
}
