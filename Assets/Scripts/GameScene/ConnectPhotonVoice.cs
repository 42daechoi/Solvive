using UnityEngine;
using Photon.Voice.PUN;
using Photon.Realtime;
using Photon.Pun;

public class ConnectPhotonVoice : MonoBehaviour
{
    void Start()
    {
        var client = PunVoiceClient.Instance.Client;

        Debug.Log("[Voice] 현재 상태: " + client.State);

        if (client.State == ClientState.Disconnected)
        {
            Debug.Log("[Voice] 클라이언트 연결 시도");
            client.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings);
        }

        client.StateChanged += OnVoiceClientStateChanged;

        // 🔥 상태가 이미 ConnectedToMasterServer일 경우 즉시 처리
        HandleVoiceClientState(client.State);
    }

    private void OnVoiceClientStateChanged(ClientState from, ClientState to)
    {
        Debug.Log($"[Voice] 상태 변경: {from} → {to}");
        HandleVoiceClientState(to);
    }

    private void HandleVoiceClientState(ClientState state)
    {
        if (state == ClientState.ConnectedToMasterServer &&
            PhotonNetwork.InRoom &&
            !PunVoiceClient.Instance.Client.InRoom)
        {
            string roomName = "Room_" + PhotonNetwork.CurrentRoom.Name + "_voice_";
            Debug.Log($"[Voice] 보이스 룸 조인 시도: {roomName}");
            PunVoiceClient.Instance.Client.OpJoinOrCreateRoom(new EnterRoomParams
            {
                RoomName = roomName
            });
        }
    }
}
