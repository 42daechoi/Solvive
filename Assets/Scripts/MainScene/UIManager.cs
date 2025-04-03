using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine;

public class UIManager : MonoBehaviourPun
{

    public GameObject GameModeSelect;
    public GameObject OptionUI;
    public GameObject ControlPanel;
    public GameObject resolutionPanel;
    public GameObject AudioPanel;
    
    public static UIManager Instance;
    
    void OnEnable()
    {
        EventManager_Main.OnFindGameClicked += FindGame;
        EventManager_Main.OnCustomGameClicked += CustomGame;
        EventManager_Main.OnCreditsClicked += Credits;
        EventManager_Main.OnOptionClicked += Option;
        EventManager_Main.OnQuitClicked += Quit;
        EventManager_Main.OnSoloModeClicked += Solo;
        EventManager_Main.OnMultiModeClicked += Multi;
        EventManager_Main.OnControlPanelButtonClicked += Control_P;
        EventManager_Main.OnResolutionPanelButtonClicked += Resolution_P;
        EventManager_Main.OnAudioPanelButtonClicked += Audio_P;
        EventManager_Main.OnOptionConfirmButtonClicked += Option_confirm;
        EventManager_Main.OnGMS_backgroundClicked += GMS_background;
    }

    void OnDisable()
    {
        EventManager_Main.OnFindGameClicked -= FindGame;
        EventManager_Main.OnCustomGameClicked -= CustomGame;
        EventManager_Main.OnCreditsClicked -= Credits;
        EventManager_Main.OnOptionClicked -= Option;
        EventManager_Main.OnQuitClicked -= Quit;
        EventManager_Main.OnSoloModeClicked -= Solo;
        EventManager_Main.OnMultiModeClicked -= Multi;
        EventManager_Main.OnControlPanelButtonClicked -= Control_P;
        EventManager_Main.OnResolutionPanelButtonClicked -= Resolution_P;
        EventManager_Main.OnAudioPanelButtonClicked -= Audio_P;
        EventManager_Main.OnOptionConfirmButtonClicked += Option_confirm;
        EventManager_Main.OnGMS_backgroundClicked += GMS_background;
    }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void FindGame()
    { 
        Debug.Log("FindGame 버튼 누름");
        // 새 게임 찾기 로직 구현 필요
        if (GameModeSelect != null)
            {
                GameModeSelect.SetActive(true);
                Debug.Log("있음");
            }
        else 
        {
            Debug.Log("없음");
        }
    }
    void GMS_background(){
        GameModeSelect.SetActive(false);
    }

    void CustomGame()
    {
        Debug.Log("CustomGame 버튼 누름");
        PhotonNetwork.LoadLevel("CustomGameScene");
        PhotonNetwork.JoinLobby();
    }

    void Credits()
    {
        SceneManager.LoadScene("CreditScene");
        Debug.Log("UIManager : Credits Button 누름");
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
    void Solo()
    {
        Debug.Log("솔로 모드 선택");
        NetworkManager.Instance.JoinRoom();
        if (GameModeSelect != null)
        {
        GameModeSelect.SetActive(false);
        }
    }
    void Multi(){
        Debug.Log("멀티 모드 선택");
        if(GameModeSelect != null)
        {
        GameModeSelect.SetActive(false);
        }
    }
    void Control_P(){
        Debug.Log("컨트롤");
        ControlPanel.SetActive(true);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(false);
    }
    void Resolution_P(){
        Debug.Log("해상도");
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(true);
        AudioPanel.SetActive(false);
    }
    void Audio_P(){
        Debug.Log("오디오");
        ControlPanel.SetActive(false);
        resolutionPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }
    void Option_confirm(){
        OptionUI.SetActive(false);
    }
}
