using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomCode : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text roomCodeText;
    [SerializeField] private Button showRoomCodeButton;
    [SerializeField] private Button copyRoomCodeButton;

    private string roomCode = "";
    private bool IsCodeVisible = false;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomCode"))
        {
            roomCode = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomCode"];
        }
        roomCodeText.text = "******";

        showRoomCodeButton.onClick.AddListener(ShowRoomCode);
        copyRoomCodeButton.onClick.AddListener(CopyRoomCodeToClipboard);
    }

    void ShowRoomCode()
    {
        if (IsCodeVisible)
        {
            roomCodeText.text = "******";
        }
        else
        {
            roomCodeText.text = roomCode;
        }
        IsCodeVisible = !IsCodeVisible;
    }

    void CopyRoomCodeToClipboard()
    {
        GUIUtility.systemCopyBuffer = roomCode;
        Debug.Log("룸 코드가 클립보드에 복사되었습니다: " + roomCode);
    }
}
