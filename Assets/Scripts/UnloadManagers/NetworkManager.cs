using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerLeftRoomText;
    public static NetworkManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Photon 서버에 연결 시도 중...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager : 포톤 마스터 서버 접속 완료");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("NetworkManager : 로비 접속 완료");
        // 방 생성 또는 입장에 필요한 준비 코드 필요
    }

    // public void CreateRoom()
    // {
    //     RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
    //     PhotonNetwork.CreateRoom("RoomName", roomOptions);
    // }

    public override void OnCreatedRoom()
    {
        Debug.Log("NetworkManager : 방 생성 완료");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("RoomName");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("NetworkManager : 방 입장 완료");
        //PhotonNetwork.LoadLevel("GameLobby");
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void BackToLobby()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "IsReady", false } });
            }

            PhotonNetwork.LoadLevel("GameLobby");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("NetworkManager : 방에서 나갔습니다.");
        SceneManager.LoadScene("MainScene");
        if (PunVoiceClient.Instance.Client.IsConnected)
        {
            PunVoiceClient.Instance.Client.Disconnect();
        }
    }


    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    if (SceneManager.GetActiveScene().name == "GameScene")
    //    {
    //        if (playerLeftRoomText != null)
    //        {
    //            photonView.RPC("SyncPlayerLeftRoomUI", RpcTarget.All, "Player ID (" + otherPlayer.UserId + ") has left the room.");

    //            StartCoroutine(ClearTextAfterDelay(3f));
    //        }
    //        else
    //        {
    //            Debug.LogError("UI 텍스트가 할당되지 않았습니다!");
    //        }
    //    }
    //}

    //private IEnumerator ClearTextAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    photonView.RPC("SyncPlayerLeftRoomUI", RpcTarget.All, "");
    //}

    //[PunRPC]
    //private void SyncPlayerLeftRoomUI(string text)
    //{
    //    playerLeftRoomText.text = text;
    //}
}
