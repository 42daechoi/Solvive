using UnityEngine;
using DG.Tweening;

public class RoleNoticeUI : MonoBehaviour
{
    public GameObject roleNoticePanel;
    public GameObject mannequinText;
    public GameObject citizenText;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = roleNoticePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = roleNoticePanel.AddComponent<CanvasGroup>();
        }

        EventManager_Game.Instance.OnSetRoleComplete += HandleRoleNoticeText;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnSetRoleComplete -= HandleRoleNoticeText;
    }

    private void HandleRoleNoticeText(PlayerRole playerRole)
    {
        Debug.Log("SetRole È£ÃâµÊ: " + playerRole);
        if (playerRole == PlayerRole.Mannequin)
        {
            mannequinText.SetActive(true);
            citizenText.SetActive(false);
        }
        else if (playerRole == PlayerRole.Citizen)
        {
            mannequinText.SetActive(false);
            citizenText.SetActive(true);
        }

        roleNoticePanel.SetActive(true);
        canvasGroup.alpha = 0.8f;

        DOVirtual.DelayedCall(10f, () =>
        {
            canvasGroup.DOFade(0f, 1.5f).OnComplete(() =>
            {
                roleNoticePanel.SetActive(false);
            });
        });
    }
}
