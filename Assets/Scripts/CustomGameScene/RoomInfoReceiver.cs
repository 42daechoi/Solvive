using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomInfoReceiver : MonoBehaviour, IOnEventCallback
{
    private RoomList roomList;

    void Awake()
    {
        // RoomList 컴포넌트가 씬에 존재한다고 가정하고 할당합니다.
        roomList = FindObjectOfType<RoomList>();
    }

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        // CreateGameRoom에서 설정한 이벤트 코드와 일치하는지 확인합니다.
        if (photonEvent.Code == CreateGameRoom.ROOM_INFO_EVENT)
        {
            object[] roomData = (object[])photonEvent.CustomData;
            string roomName = (string)roomData[0];
            byte maxPlayers = (byte)roomData[1];
            int currentPlayers = (int)roomData[2];
            bool isPrivate = (bool)roomData[3];
            

            // 예를 들어, RoomList에 커스텀 메서드를 추가해 받은 방 정보를 갱신하도록 할 수 있습니다.
            if (roomList != null)
            {
                roomList.AddOrUpdateRoomInfo(roomName, maxPlayers, currentPlayers, isPrivate);
            }
            else
            {
                Debug.LogWarning("RoomList 인스턴스가 할당되지 않았습니다.");
            }
        }
    }
}