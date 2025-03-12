using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EscUI : MonoBehaviourPunCallbacks
{
    public static event Action OnBackGameClicked;
    public static event Action OnExitGameClicked;

    public GameObject escCanvas; // 캔버스 참조 (Hierarchy에서 할당)
    public Button backGameButton; // 백게임 버튼 참조
    public Button exitGameButton; // 게임 종료 버튼 참조
    private bool esctoogle = false;

    void Start()
    {
        // 시작할 때 캔버스를 비활성화
        escCanvas.SetActive(false);

        // 버튼 클릭 이벤트 연결
        backGameButton.onClick.AddListener(BackGame);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    void OnEnable()
    {
        EventManager_Game.OnEscButton += ShowEscUI;
    }

    void OnDisable()
    {
        EventManager_Game.OnEscButton -= ShowEscUI;
    }

    void ShowEscUI()
    {
        Debug.Log("ESC key pressed from event");
        if (esctoogle == true)
        {
            escCanvas.SetActive(true);
            esctoogle = false;
        }
        else
        {
            escCanvas.SetActive(false);
            esctoogle = true;
        }
    }

    // 백게임 버튼 클릭 시 호출되는 메서드
    public void BackGame()
    {
        escCanvas.SetActive(false);
        OnBackGameClicked?.Invoke();
    }

    // 종료 버튼 클릭 시 호출되는 메서드
    public void ExitGame()
    {
        OnExitGameClicked?.Invoke();
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("onleftroom확인");
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
