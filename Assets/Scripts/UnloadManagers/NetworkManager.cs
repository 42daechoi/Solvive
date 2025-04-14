using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
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
        if (SteamManager.Initialized)
        {
            string steamNickname = SteamManager.GetSteamNickname();
            PhotonNetwork.NickName = steamNickname;
            Debug.Log("스팀 닉네임으로 Photon 닉네임 설정: " + steamNickname);
        }
        else
        {
            PhotonNetwork.NickName = "Guest_" + Random.Range(1000, 9999);
            Debug.LogWarning("SteamManager 초기화 실패. 임시 닉네임 설정");
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Photon 서버에 연결 시도 중...");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("NetworkManager : 포톤 마스터 서버 접속 완료");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("NetworkManager : 로비 접속 완료");
        GameLoadingScene.Instance.CheckCountOfPlayer();
        //여기서 스팀 닉네임을 들고 와야 할듯
        // if (SteamManager.Initialized)
        // {
        //     string steamNickname = SteamFriends.GetPersonaName();
        //     Debug.Log("스팀 닉네임: " + steamNickname);
        //     PhotonNetwork.NickName = steamNickname;
        // }
    }

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
        StartCoroutine(LeaveRoomAndWaitVoiceDisconnect());
    }

    private IEnumerator LeaveRoomAndWaitVoiceDisconnect()
    {
        if (PunVoiceClient.Instance.Client.IsConnected)
        {
            Debug.Log("NetworkManager : 포톤보이스 디스커넥");
            PunVoiceClient.Instance.Client.Disconnect();

            while (PunVoiceClient.Instance.Client.IsConnected)
            {
                yield return null;
            }
        }

        Debug.Log("VoiceClient 연결 해제 완료 → 씬 이동");
        SceneManager.LoadScene("MainScene");
        if (PunVoiceClient.Instance != null)
        {
            Destroy(PunVoiceClient.Instance.gameObject);
        }
    }

}
