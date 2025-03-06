using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndGameUI : MonoBehaviour
{
    public GameObject endGameCanvasObject;
    public TextMeshProUGUI winnerText;
    public GameObject inventoryCanvasObject;
    public Image buttonImage;

    private void Start()
    {
        EventManager_Game.Instance.OnEndGame += ActiveEndGameUI;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnEndGame -= ActiveEndGameUI;
    }

    private void ActiveEndGameUI(PlayerRole playerRole)
    {
        Debug.Log("EndGameUI : 게임 종료 UI 활성화");
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
    }
}
