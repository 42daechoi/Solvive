using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameLobbyManager : MonoBehaviourPunCallbacks
{
    public Transform playerListContent;
    public GameObject playerItemPrefab;
    public Button actionButton;
    public TMP_Text actionButtonText;
    public Button backButton;
    public Canvas LoadingCanvas;
    public RoomList roomList;

    private Dictionary<string, GameObject> playerItems = new Dictionary<string, GameObject>();

    private void Start()
    {
        backButton.onClick.AddListener(LeaveRoom);
        actionButton.onClick.AddListener(OnActionButtonClicked);
        UpdateActionButton();
        UpdatePlayerList();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateActionButton();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            UpdatePlayerList();
            UpdateActionButton();
        }
    }

    private void UpdatePlayerList()
    {
        if (playerListContent == null)
        {
            return;
        }

        if (playerItemPrefab == null)
        {
            return;
        }

        // 기존 PlayerCard 삭제
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 PlayerCard 추가
        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerItemPrefab, playerListContent);

            if (playerItem == null)
            {
                continue;
            }

            // 이름 업데이트
            TMP_Text nameText = playerItem.transform.Find("PlayerName")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = player.NickName;
            }

            // Ready 상태 업데이트
            TMP_Text readyStatusText = playerItem.transform.Find("ReadyStatus")?.GetComponent<TMP_Text>();
            if (readyStatusText != null)
            {
                object isReady;
                
                if (player.IsMasterClient)
                {
                    readyStatusText.text = "RoomMaster";
                }
                else if (player.CustomProperties.TryGetValue("IsReady", out isReady) && (bool)isReady)
                {
                    readyStatusText.text = "Ready";
                }
                else
                {
                    readyStatusText.text = "not ready";
                }
            }

            // Kick 버튼 설정 (방장만 활성화)
            Button kickButton = playerItem.transform.Find("KickButton")?.GetComponent<Button>();
            if (kickButton != null)
            {
                kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !player.IsMasterClient);
                kickButton.onClick.AddListener(() => KickPlayer(player));
            }
        }
    }

    private void UpdateActionButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            actionButtonText.text = "Start";

            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                object isReady;
                if (player.CustomProperties.TryGetValue("IsReady", out isReady) && !(bool)isReady && !player.IsMasterClient)
                {
                    allReady = false;
                    break;
                }
            }

            actionButton.interactable = allReady;
        }
        else
        {
            actionButtonText.text = "Ready";
            actionButton.interactable = true;
        }
    }

    private void OnActionButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
        else
        {
            ToggleReadyStatus();
        }
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        
    }

    private void StartGame()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            object isReady;
            if (player.CustomProperties.TryGetValue("IsReady", out isReady) && !(bool)isReady && !player.IsMasterClient)
            {
                return;
            }
        }

        PhotonNetwork.CurrentRoom.IsOpen = false; //방 입장 기능 비활성화
        PhotonNetwork.CurrentRoom.IsVisible = false;    //방 목록 비가시화
        
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();  //프롬포티 선언
        props["GameStarted"] = true;    //프롬포티에 방 시작 true로 변경
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);   //방 설정 업데이트
        
        photonView.RPC("Loading", RpcTarget.All);
        Debug.Log("게임 시작");
        PhotonNetwork.LoadLevel("GameScene");
    }
    
    public override void OnJoinedRoom()
    {
        // 플레이어의 IsReady 상태를 초기화 (not ready 상태)
        Hashtable props = new Hashtable
        {
            { "IsReady", false } // 기본값을 false로 설정
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // UI 업데이트
        UpdatePlayerList();
        UpdateActionButton();
    }


    private void ToggleReadyStatus()
    {
        bool isReady = false;
        object readyState;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("IsReady", out readyState))
        {
            isReady = !(bool)readyState;
        }

        Hashtable props = new Hashtable
        {
            { "IsReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void KickPlayer(Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("KickPlayerRPC", player);
        }
    }
    
    [PunRPC]
    public void KickPlayerRPC()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainScene");
    }
    
    [PunRPC]
    public void Loading()
    {
        LoadingCanvas.gameObject.SetActive(true);
    }
    
}
