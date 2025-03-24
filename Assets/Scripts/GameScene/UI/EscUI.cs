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
    public static event Action OnSettingClicked;
    public static event Action OnControlPanelButtonClicked;
    public static event Action OnResolutionPanelButtonClicked;
    public static event Action OnAudioPanelButtonClicked;
    public static event Action OnOptionConfirmButtonClicked;

    public GameObject escCanvas; // 캔버스 참조 (Hierarchy에서 할당)
    public Button backGameButton; // 백게임 버튼 참조
    public Button exitGameButton; // 게임 종료 버튼 참조
    public Button SettingButton;
    public GameObject OptionUI;
    private bool esctoogle = false;
    public GameObject ControlPanel;
    public GameObject resolutionPanel;
    public GameObject AudioPanel;

    void Start()
    {
        // 시작할 때 캔버스를 비활성화
        escCanvas.SetActive(false);
        
        backGameButton.onClick.AddListener(BackGame);
        exitGameButton.onClick.AddListener(ExitGame);
        SettingButton.onClick.AddListener(OptionSetting);
        
        if (ControlPanel == null)
        {
            ControlPanel = GameObject.Find("ControlPanel");
        }

        if (resolutionPanel == null)
        {
            resolutionPanel = GameObject.Find("ResolutionPanel");
        }

        if (AudioPanel == null)
        {
            AudioPanel = GameObject.Find("AudioPanel");
        }
    }

    void OnEnable()
    {
        EventManager_Game.OnEscButton += ShowEscUI;
        EventManager_Game.OnControlPanelButtonClicked += ControlPanelButton;
        EventManager_Game.OnResolutionPanelButtonClicked += ResolutionPanelButton;
        EventManager_Game.OnAudioPanelButtonClicked += AudioPanelButton;
        EventManager_Game.OnOptionConfirmButtonClicked += OptionConfirmButton;
    }

    void OnDisable()
    {
        EventManager_Game.OnEscButton -= ShowEscUI;
        EventManager_Game.OnControlPanelButtonClicked -= ControlPanelButton;
        EventManager_Game.OnResolutionPanelButtonClicked -= ResolutionPanelButton;
        EventManager_Game.OnAudioPanelButtonClicked -= AudioPanelButton;
        EventManager_Game.OnOptionConfirmButtonClicked -= OptionConfirmButton;
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

    public void OptionSetting()
    {
        OnSettingClicked?.Invoke();
        OptionUI.SetActive(true);
    }

    public void ControlPanelButton()
    {
        ControlPanel.SetActive(true);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(false);
    }

    public void ResolutionPanelButton()
    {
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(true);
        AudioPanel.SetActive(false);
    }

    public void AudioPanelButton()
    {
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }

    public void OptionConfirmButton()
    {
        OptionUI.SetActive(false);
    }
}
