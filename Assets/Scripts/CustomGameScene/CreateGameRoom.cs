using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CreateGameRoom : MonoBehaviourPunCallbacks
{
    public Toggle publicToggle;
    public Toggle privateToggle;
    public TMP_InputField numberOfPeopleInput;
    public Button confirmButton;
    public Canvas GameRobbyCanvas;
    public const byte ROOM_INFO_EVENT = 1;


    public void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 연결
            Debug.Log("Photon 서버에 연결 시도 중...");
        }
        confirmButton.onClick.AddListener(CreateRoom);
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 참가 실패: {message}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"포톤 서버 연결 끊김: {cause}");
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();

        // Public/Private 설정
        roomOptions.IsVisible = publicToggle.isOn;
        roomOptions.IsOpen = true;

        // 최대 인원 설정
        int maxPlayers;
        if (int.TryParse(numberOfPeopleInput.text, out maxPlayers))
        {
            roomOptions.MaxPlayers = (byte)maxPlayers;
        }
        // 방 이름 생성 및 방 생성
        string roomName = "Room_" + Random.Range(1000, 10000);
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"방 생성 시도: {roomName}");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"방 생성 성공: {PhotonNetwork.CurrentRoom.Name}");
        object[] roomData = new object[]
        {
            PhotonNetwork.CurrentRoom.Name,
            PhotonNetwork.CurrentRoom.MaxPlayers,
            PhotonNetwork.CurrentRoom.PlayerCount,
            PhotonNetwork.CurrentRoom.IsVisible
        };

        // 모든 클라이언트에게 전송할 옵션 설정 (모두에게)
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(ROOM_INFO_EVENT, roomData, raiseEventOptions, sendOptions);
        Debug.Log("방 정보 업로드 이벤트 전송 완료");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 참가 성공: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel("GameLobby"); // 방 참가 성공 시 GameLobby로 이동
    }
    
}
