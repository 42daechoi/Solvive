using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviourPun
{
    public GameObject OptionUI;
    public GameObject ControlPanel;
    public GameObject resolutionPanel;
    public GameObject AudioPanel;
    public GameObject HowToPlayPanel;
    
    public static UIManager Instance;
    
    void OnEnable()
    {
        EventManager_Main.OnCustomGameClicked += CustomGame;
        EventManager_Main.OnCreditsClicked += Credits;
        EventManager_Main.OnOptionClicked += Option;
        EventManager_Main.OnQuitClicked += Quit;
        EventManager_Main.OnControlPanelButtonClicked += Control_P;
        EventManager_Main.OnResolutionPanelButtonClicked += Resolution_P;
        EventManager_Main.OnAudioPanelButtonClicked += Audio_P;
        EventManager_Main.OnOptionConfirmButtonClicked += Option_confirm;
        EventManager_Main.OnHowToPlayClicked += HowToPlay;
        EventManager_Main.OnBackClicked += Back;
    }

    void OnDisable()
    {
        EventManager_Main.OnCustomGameClicked -= CustomGame;
        EventManager_Main.OnCreditsClicked -= Credits;
        EventManager_Main.OnOptionClicked -= Option;
        EventManager_Main.OnQuitClicked -= Quit;
        EventManager_Main.OnControlPanelButtonClicked -= Control_P;
        EventManager_Main.OnResolutionPanelButtonClicked -= Resolution_P;
        EventManager_Main.OnAudioPanelButtonClicked -= Audio_P;
        EventManager_Main.OnOptionConfirmButtonClicked -= Option_confirm;
        EventManager_Main.OnHowToPlayClicked -= HowToPlay;
        EventManager_Main.OnBackClicked -= Back;
    }

    void CustomGame()
    {
        PhotonNetwork.LoadLevel("CustomGameScene");
        PhotonNetwork.JoinLobby();
    }

    void Credits()
    {
        SceneManager.LoadScene("CreditScene");
    }

    void Option()
    {
        Debug.Log("Option 버튼 누름");
        OptionUI.SetActive(true);
    }

    void Quit()
    {
        Debug.Log("Quit 버튼 누름");
    }
    void Control_P(){
        ControlPanel.SetActive(true);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(false);
    }
    void Resolution_P(){
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(true);
        AudioPanel.SetActive(false);
    }
    void Audio_P(){
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }
    void Option_confirm(){
        OptionUI.SetActive(false);
    }
    void HowToPlay()
    {
        HowToPlayPanel.SetActive(true);
    }

    void Back()
    {
        HowToPlayPanel.SetActive(false);
    }
    
}
