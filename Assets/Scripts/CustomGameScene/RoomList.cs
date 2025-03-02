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

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Button lastSelectedButton; // 마지막으로 선택된 버튼

    public RoomInfo selectedRoom;
    public CustomUI_Event customUIEvent;
    public RoomList roomList;
    public event Action<RoomInfo> OnRoomSelected;

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                // 리스트에서 제거된 방은 삭제
                cachedRoomList.Remove(roomInfo.Name);
            }
            else
            {
                // 새 방 추가 또는 기존 방 정보 업데이트
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        // 기존 룸 리스트 아이템 제거
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        // 룸 리스트 재생성
        foreach (var roomEntry in cachedRoomList)
        {
            RoomInfo room = roomEntry.Value;

            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            // 방 이름
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;

            // 방의 게임 모드 표시
            // if (room.CustomProperties.ContainsKey("GameMode"))
            // {
            //     string gameMode = (string)room.CustomProperties["GameMode"];
            //     roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gameMode;  // 두 번째 항목에 게임 모드 표시
            // }

            // 플레이어 수 / 최대 플레이어 수 표시
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;

            Button button = roomItem.GetComponent<Button>();

            // 버튼 클릭 이벤트 추가
            button.onClick.AddListener(() => SelectRoom(room, button));
        }
    }

    public void SelectRoom(RoomInfo roomInfo, Button clickedButton)
    {
        selectedRoom = roomInfo;
        Debug.Log("선택된 방: " + selectedRoom.Name);

        // 이전 버튼 색상 초기화
        if (lastSelectedButton != null)
        {
            lastSelectedButton.image.color = defaultColor;
        }

        // 현재 버튼 강조
        clickedButton.image.color = selectedColor;
        lastSelectedButton = clickedButton;

        // 방 선택 이벤트 호출
        OnRoomSelected?.Invoke(roomInfo); // 이벤트 호출

        // 선택한 방 정보를 CustomUI_Event에 전달
        if (customUIEvent != null)
        {
            customUIEvent.OnRoomButtonClicked(roomInfo);
        }
    }
    
    public void OnClick_RefreshButton()
    {
        Debug.Log("방 목록 새로 고침 버튼 클릭!");

        // 1) 캐시된 리스트 초기화 (혹은 필요에 따라 유지)
        cachedRoomList.Clear();

        // 2) 현재 로비에 있다면 나가기
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        // 3) 다시 로비에 입장 (이후 OnRoomListUpdate가 호출되며, UpdateUI()가 실행됨)
        PhotonNetwork.JoinLobby();
    }

}
