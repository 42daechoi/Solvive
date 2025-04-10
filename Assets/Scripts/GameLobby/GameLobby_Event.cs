using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

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
        Debug.Log($"플레이어 입장: {newPlayer.NickName}");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 퇴장: {otherPlayer.NickName}");
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"방장 변경: {newMasterClient.NickName}");
        UpdateActionButton();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            Debug.Log($"플레이어 {targetPlayer.NickName} 준비 상태 변경: {changedProps["IsReady"]}");
            UpdatePlayerList();
            UpdateActionButton();
        }
    }

    private void UpdatePlayerList()
    {
        if (playerListContent == null)
        {
            Debug.LogError("playerListContent가 null입니다. Inspector에서 Content 오브젝트를 연결하세요.");
            return;
        }

        if (playerItemPrefab == null)
        {
            Debug.LogError("playerItemPrefab이 null입니다. Inspector에서 PlayerCard Prefab을 연결하세요.");
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
                Debug.LogError("PlayerItem Prefab이 null입니다.");
                continue;
            }

            // 이름 업데이트
            TMP_Text nameText = playerItem.transform.Find("PlayerName")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = player.NickName;
            }
            else
            {
                Debug.LogError("PlayerName 텍스트를 찾을 수 없습니다. PlayerCard Prefab 구조를 확인하세요.");
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
            else
            {
                Debug.LogError("ReadyStatus 텍스트를 찾을 수 없습니다. PlayerCard Prefab 구조를 확인하세요.");
            }

            // Kick 버튼 설정 (방장만 활성화)
            Button kickButton = playerItem.transform.Find("KickButton")?.GetComponent<Button>();
            if (kickButton != null)
            {
                kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !player.IsMasterClient);
                kickButton.onClick.AddListener(() => KickPlayer(player));
            }
            else
            {
                Debug.LogError("KickButton을 찾을 수 없습니다. PlayerCard Prefab 구조를 확인하세요.");
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
        Debug.Log("방 나가기");
        PhotonNetwork.LeaveRoom();
        
    }

    private void StartGame()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            object isReady;
            if (player.CustomProperties.TryGetValue("IsReady", out isReady) && !(bool)isReady && !player.IsMasterClient)
            {
                Debug.LogWarning($"플레이어 {player.NickName}가 준비되지 않았습니다.");
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
        Debug.Log("방에 입장했습니다.");

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
            PhotonNetwork.CloseConnection(player);
            Debug.Log($"플레이어 강퇴: {player.NickName}");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByServerLogic)
        {
            Debug.Log("강퇴되었습니다. 메인 화면으로 돌아갑니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
        else
        {
            Debug.Log($"연결이 끊겼습니다: {cause}");
        }
    }

    [PunRPC]
    public void Loading()
    {
        LoadingCanvas.gameObject.SetActive(true);
    }
    
}
