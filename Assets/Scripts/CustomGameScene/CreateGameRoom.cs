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

	private List<RoomInfo> cachedRoomList = new List<RoomInfo>();


	public void Start()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 연결
		}
		confirmButton.onClick.AddListener(CreateRoom);
	}
	
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		
	}

	public void CreateRoom()
	{
		RoomOptions roomOptions = new RoomOptions();

		// 방코드 설정
		string roomCode = GenerateRoomCode();
		while (!IsNotDuplicated(roomCode))
		{
			roomCode = GenerateRoomCode();
		}

		// 최대 인원 설정
		int maxPlayers;
		if (int.TryParse(numberOfPeopleInput.text, out maxPlayers))
		{
			roomOptions.MaxPlayers = (byte)maxPlayers;
		}

		// Public/Private 설정, RoomCode 설정
		Hashtable customProps = new Hashtable
		{
			{ "IsPrivate", privateToggle.isOn },
			{ "RoomCode", roomCode },
		};
		roomOptions.CustomRoomProperties = customProps;
		roomOptions.CustomRoomPropertiesForLobby = new string[] { "IsPrivate", "RoomCode" };


		// 방 이름 생성 및 방 생성
		string roomName = "Room_" + Random.Range(1000, 10000);
		PhotonNetwork.CreateRoom(roomName, roomOptions);
		Debug.Log($"방 생성 시도: {roomName}, 코드: {roomCode}");
	}

	private bool IsNotDuplicated(string roomCode)
	{
		foreach (RoomInfo room in cachedRoomList)
		{
			if (room.CustomProperties.ContainsKey("RoomCode"))
			{
				if (room.CustomProperties["RoomCode"].ToString() == roomCode)
				{
					return false;
				}
			}
		}
		return true;
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		cachedRoomList = roomList;
	}

	private string GenerateRoomCode()
	{
		return Random.Range(100000, 999999).ToString();
	}

	public override void OnCreatedRoom()
	{
		Debug.Log($"방 생성 성공: {PhotonNetwork.CurrentRoom.Name}");
		bool isPrivate = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsPrivate"))
        {
            isPrivate = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsPrivate"];
        }
        object[] roomData = new object[]
		{
			PhotonNetwork.CurrentRoom.Name,
			PhotonNetwork.CurrentRoom.MaxPlayers,
			PhotonNetwork.CurrentRoom.PlayerCount,
			isPrivate
		};

		// 모든 클라이언트에게 전송할 옵션 설정 (모두에게)
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
		SendOptions sendOptions = new SendOptions { Reliability = true };

		PhotonNetwork.RaiseEvent(ROOM_INFO_EVENT, roomData, raiseEventOptions, sendOptions);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("GameLobby");
	}
	
}
