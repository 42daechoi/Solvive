using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class CustomUI_Event : MonoBehaviourPunCallbacks
{
    public GameObject CreateGameRoom;
    public TMP_InputField NumOfPeople;
    public GameObject joinWithCodePanel;
    public TMP_InputField roomCodeInput;
    public int step = 1;
    public int minValue = 2;  // 최소 인원
    public int maxValue = 16;  // 최대 인원
    private int currentValue = 0; // 방 인원
    private RoomList.CustomRoomInfo selectedRoom;
    RoomList roomListComponent;

    public override void OnEnable()
    {
        EventManager_Custom.OnCreateButtonClicked += Create;
        EventManager_Custom.OnJoinButonClicked += Join;
        EventManager_Custom.OnSearchButtonClicked += Search;
        EventManager_Custom.OnPreviousButtonClicked += Previous;
        EventManager_Custom.OnIncreaseButtonClicked += Increase;
        EventManager_Custom.OnDecreaseButtonClicked += Decrease;
        EventManager_Custom.OnCancleButtonClicked += Cancle;
        EventManager_Custom.OnCreateComfirmButtonClicked += CreateComfirm;
        EventManager_Custom.OnJoinWithCodeButtonClicked += HandleJoinWithCode;
        EventManager_Custom.OnJoinWithCodeEscButtonClicked += HandleJoinWithCodeEsc;
        EventManager_Custom.OnCodeJoinButtonClicked += HandleCodeJoin;
        roomListComponent = FindObjectOfType<RoomList>();
        if (roomListComponent != null)
        {
            roomListComponent.OnRoomSelected += OnRoomButtonClicked; // 방 선택 시 처리
        }
    }

    public override void OnDisable()
    {
        EventManager_Custom.OnCreateButtonClicked -= Create;
        EventManager_Custom.OnJoinButonClicked -= Join;
        EventManager_Custom.OnSearchButtonClicked -= Search;
        EventManager_Custom.OnPreviousButtonClicked -= Previous;
        EventManager_Custom.OnIncreaseButtonClicked -= Increase;
        EventManager_Custom.OnDecreaseButtonClicked -= Decrease;
        EventManager_Custom.OnCancleButtonClicked -= Cancle;
        EventManager_Custom.OnCreateComfirmButtonClicked -= CreateComfirm;
        EventManager_Custom.OnJoinWithCodeButtonClicked -= HandleJoinWithCode;
        EventManager_Custom.OnJoinWithCodeEscButtonClicked -= HandleJoinWithCodeEsc;
        EventManager_Custom.OnCodeJoinButtonClicked -= HandleCodeJoin;
        RoomList roomListComponent = FindObjectOfType<RoomList>();
        if (roomListComponent != null)
        {
            roomListComponent.OnRoomSelected -= OnRoomButtonClicked; // 방 선택 시 처리
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings(); // Photon 서버에 연결
        }

        if (NumOfPeople.text != "")
        {
            currentValue = int.Parse(NumOfPeople.text);
        }
        else
        {
            currentValue = minValue;
            NumOfPeople.text = currentValue.ToString();
        }
    }

    void Create()
    {
        if (CreateGameRoom != null)
        {
            CreateGameRoom.SetActive(true);
        }
        Debug.Log("create버튼");
    }

    public void OnRoomButtonClicked(RoomList.CustomRoomInfo roomInfo)
    {
        selectedRoom = roomInfo; // 선택된 방 정보 저장
        Debug.Log("선택된 방: " + selectedRoom.RoomName);
    }

    void Join()
    {
        if (selectedRoom != null)
        {
            PhotonNetwork.JoinRoom(selectedRoom.RoomName); // 선택된 방 입장
            Debug.Log("방에 입장: " + selectedRoom.RoomName);
        }
        else
        {
            Debug.Log("선택된 방이 없습니다.");
        }
        Debug.Log("join버튼");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 참가 성공: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel("GameLobby");
    }

    void Search()
    {
        Debug.Log("search버튼");
        // Search 버튼 로직 구현 필요
    }

    void Previous()
    {
        Debug.Log("previous버튼");
        SceneManager.LoadScene("MainScene");
    }

    void Increase()
    {
        currentValue += step;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        NumOfPeople.text = currentValue.ToString();
    }

    void Decrease()
    {
        currentValue -= step;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
        NumOfPeople.text = currentValue.ToString();
    }

    public void OnInputFieldChanged()
    {
        if (NumOfPeople.text != "")
        {
            currentValue = int.Parse(NumOfPeople.text);
            currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
            NumOfPeople.text = currentValue.ToString();
        }
    }

    public void Cancle()
    {
        if (CreateGameRoom != null)
        {
            CreateGameRoom.SetActive(false);
        }
    }

    private void HandleJoinWithCode()
    {
        joinWithCodePanel.SetActive(true);
        roomCodeInput.text = "";
    }

    private void HandleJoinWithCodeEsc()
    {
        joinWithCodePanel.SetActive(false);
    }

    private void HandleCodeJoin()
    {
        int count = 0;
        string enteredCode = roomCodeInput.text.Trim();
        if (string.IsNullOrEmpty(enteredCode))
        {
            return;
        }

        foreach (RoomInfo room in roomListComponent.roomInfoList)
        {
            if (room.CustomProperties.ContainsKey("RoomCode") &&
                room.CustomProperties["RoomCode"].ToString() == enteredCode)
            {
                PhotonNetwork.JoinRoom(room.Name);
                Debug.Log($"CustomUI_Event : 방 코드 {enteredCode}에 해당하는 방({room.Name}) 입장 시도");
                return;
            }
            count++;
        }

        Debug.Log($"CustomUI_Event : 해당 방 코드를 가진 방이 없습니다. 방숫자{count}.");
    }

    public void CreateComfirm()
    {
        Debug.Log("방 생성 완료");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공! GameLobby로 이동");
        PhotonNetwork.LoadLevel("GameLobby"); // GameLobby 씬으로 이동
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 퇴장");
    }
}
