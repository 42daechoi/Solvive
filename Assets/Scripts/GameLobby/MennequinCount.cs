using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class MannequinCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private Button increaseButton;
    private int mannequinCount = 1;

    private void Start()
    {
        UpdateUI();

        decreaseButton.onClick.AddListener(DecreaseMannequinCount);
        increaseButton.onClick.AddListener(IncreaseMannequinCount);

        if (PhotonNetwork.IsMasterClient)
        {
            SetMannequinCount(mannequinCount);
        }
    }

    private void IncreaseMannequinCount()
    {
        int maxMannequinCount = GetMaxMannequinCount();
        if (mannequinCount < maxMannequinCount)
        {
            mannequinCount++;
            SetMannequinCount(mannequinCount);
        }
    }

    private void DecreaseMannequinCount()
    {
        if (mannequinCount > 1)
        {
            mannequinCount--;
            SetMannequinCount(mannequinCount);
        }
    }

    public void SetMannequinCount(int count)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Hashtable roomProperties = new Hashtable();
        roomProperties["MannequinCount"] = count;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("MannequinCount"))
        {
            mannequinCount = (int)propertiesThatChanged["MannequinCount"];
            UpdateUI();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int maxMannequinCount = GetMaxMannequinCount();

            if (mannequinCount > maxMannequinCount)
            {
                SetMannequinCount(maxMannequinCount);
            }
        }
    }

    private void UpdateUI()
    {
        countText.text = mannequinCount.ToString();
    }

    private int GetMaxMannequinCount()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;

        if (playerCount <= 2)
        {
            return 1;
        }
        return Mathf.Max(1, playerCount / 2);
    }
}