using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class RoomList : MonoBehaviourPunCallbacks
{
	[Header("UI")] 
	public Transform roomListParent; // 방 리스트 부모 오브젝트
	public GameObject roomListItemPrefab; // 방 아이템 프리팹
	public Color selectedColor = Color.green; // 선택된 버튼의 강조 색상
	public Color defaultColor = Color.white; // 기본 버튼 색상

	// CustomRoomInfo를 저장하는 캐시 딕셔너리
	private Dictionary<string, CustomRoomInfo> cachedRoomList = new Dictionary<string, CustomRoomInfo>();
	private Button lastSelectedButton; // 마지막으로 선택된 버튼

	public CustomRoomInfo selectedRoom;
	public CustomUI_Event customUIEvent;
	public event Action<CustomRoomInfo> OnRoomSelected;

	public List<RoomInfo> roomInfoList;

	// 커스텀 룸 정보 클래스
	public class CustomRoomInfo
	{
		public string RoomName;
		public byte MaxPlayers;
		public int CurrentPlayers;
		public bool IsPrivate;
	}

	IEnumerator Start()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom();
		}

		yield return new WaitUntil(() => !PhotonNetwork.InRoom);

		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
		}
	}
	
	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();
		PhotonNetwork.JoinLobby();
	}

	// 로비에 있을 때 자동으로 호출되는 콜백
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		bool isPrivate = false;
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.RemovedFromList)
			{
				// 리스트에서 제거된 방은 캐시에서 삭제
				cachedRoomList.Remove(roomInfo.Name);
			}
			else
			{
				if (roomInfo.CustomProperties.ContainsKey("IsPrivate"))
				{
					isPrivate = (bool)roomInfo.CustomProperties["IsPrivate"];
				}
				// 새 방 추가 또는 기존 방 정보 업데이트
				CustomRoomInfo info = new CustomRoomInfo()
				{
					RoomName = roomInfo.Name,
					MaxPlayers = (byte)roomInfo.MaxPlayers,
					CurrentPlayers = roomInfo.PlayerCount,
					IsPrivate = isPrivate,
				};
				cachedRoomList[roomInfo.Name] = info;
			}
		}
		UpdateUI();
		roomInfoList = roomList;
	}

	public void UpdateUI()
	{
		// 기존 룸 리스트 아이템 제거
		foreach (Transform roomItem in roomListParent)
		{
			Destroy(roomItem.gameObject);
		}

		// 캐시된 CustomRoomInfo를 기반으로 UI 재생성
		foreach (var kvp in cachedRoomList)
		{
			CustomRoomInfo room = kvp.Value;
			
			if (room.IsPrivate == false)
			{
				GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

				// 첫 번째 자식: 방 이름 표시
				roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.RoomName;
				// 두 번째 자식: 플레이어 수 / 최대 플레이어 수 표시
				roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.CurrentPlayers + "/" + room.MaxPlayers;

				Button button = roomItem.GetComponent<Button>();
				// 버튼 클릭 이벤트 추가
				button.onClick.AddListener(() => SelectRoom(room, button));
			}
		}
	}

	public void SelectRoom(CustomRoomInfo roomInfo, Button clickedButton)
	{
		selectedRoom = roomInfo;
		Debug.Log("선택된 방: " + selectedRoom.RoomName);

		// 이전 버튼 색상 초기화
		if (lastSelectedButton != null)
		{
			lastSelectedButton.image.color = defaultColor;
		}
		// 현재 버튼 강조
		clickedButton.image.color = selectedColor;
		lastSelectedButton = clickedButton;

		// 이벤트와 CustomUI_Event에 선택 정보를 전달
		OnRoomSelected?.Invoke(roomInfo);
		customUIEvent?.OnRoomButtonClicked(roomInfo);
	}
	
	public void OnClick_RefreshButton()
	{
		Debug.Log("방 목록 새로 고침 버튼 클릭!");

		// 캐시 초기화 후 로비 재가입 (이후 OnRoomListUpdate 호출)
		cachedRoomList.Clear();
		if (PhotonNetwork.InLobby)
		{
			PhotonNetwork.LeaveLobby();
		}
		PhotonNetwork.JoinLobby();
	}

	// 커스텀 이벤트(또는 RPC)로 받은 방 정보를 갱신하는 메서드
	public void AddOrUpdateRoomInfo(string roomName, byte maxPlayers, int currentPlayers, bool isVisible)
	{
		if (cachedRoomList.ContainsKey(roomName))
		{
			// 기존 정보를 업데이트
			CustomRoomInfo info = cachedRoomList[roomName];
			info.MaxPlayers = maxPlayers;
			info.CurrentPlayers = currentPlayers;
		}
		else
		{
			// 신규 방 정보 추가
			CustomRoomInfo info = new CustomRoomInfo()
			{
				RoomName = roomName,
				MaxPlayers = maxPlayers,
				CurrentPlayers = currentPlayers,
				IsPrivate = isVisible
			};
			cachedRoomList.Add(roomName, info);
		}
		UpdateUI();
	}
}
