using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;


public class EndGameUI : MonoBehaviour
{
    public GameObject endGameCanvasObject;
    public TextMeshProUGUI winnerText;
    public GameObject inventoryCanvasObject;
    public Image buttonImage;
    public Button backToLobby;
    public Button leaveGame;
    public TextMeshProUGUI waitForMasterText;

    private void Start()
    {
        EventManager_Game.Instance.OnEndGame += ActiveEndGameUI;

        backToLobby.onClick.AddListener(OnClickBackToLobby);
        leaveGame.onClick.AddListener(OnClickLeaveGame);
        SwitchButtonOrText();
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnEndGame -= ActiveEndGameUI;

        backToLobby.onClick.RemoveListener(OnClickBackToLobby);
        leaveGame.onClick.RemoveListener(OnClickLeaveGame);
    }
    
    private void SwitchButtonOrText()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            backToLobby.gameObject.SetActive(true);
            waitForMasterText.gameObject.SetActive(false);
        }
        else
        {
            backToLobby.gameObject.SetActive(false);
            waitForMasterText.gameObject.SetActive(true);
        }
    }

    private void ActiveEndGameUI(PlayerRole playerRole)
    {
        Debug.Log("EndGameUI : ���� ���� UI Ȱ��ȭ");
        if (playerRole == PlayerRole.Mannequin)
        {
            winnerText.text = "Mannequin Win!";
        }
        else
        {
            winnerText.text = "Civilian Win!";
        }
        inventoryCanvasObject.SetActive(false);
        endGameCanvasObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickBackToLobby()
    {
        NetworkManager.Instance.BackToLobby();
    }


    public void OnClickLeaveGame()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
