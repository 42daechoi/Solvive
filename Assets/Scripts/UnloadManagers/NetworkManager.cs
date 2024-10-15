using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
        // 포톤 서버 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤 마스터 서버 접속 완료");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료");
        // 방 생성 또는 입장에 필요한 준비 코드 필요
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom("RoomName", roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom("RoomName");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        // 플레이어 캐릭터 인스턴스화 과정 필요
        // PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity, 0);
    }

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    Debug.Log(otherPlayer.NickName + "님이 퇴장하셨습니다.");
    //}
    [PunRPC]
    public void YourRpcMethod(string message)
    {
        Debug.Log("Received RPC message: " + message);
    }

    // 다른 메서드에서 RPC 호출
    public void CallRpc()
    {
        photonView.RPC("YourRpcMethod", RpcTarget.All, "Hello, everyone!");
    }
}
