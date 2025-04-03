using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Hatch : MonoBehaviourPun, IInteractableObject
{
    private bool isHolding = false;
    private float holdTime = 0f;
    private float holdDuration = 5f;
    private int interactingPlayerID = -1;

    private GameObject hatchPanel;
    private Slider holdProgressBar;

    private void Start()
    {
        GameObject uiRoot = GameObject.Find("UI");
        if (uiRoot != null)
        {
            hatchPanel = FindChild(uiRoot.transform, "HatchInteractPanel")?.gameObject;
            if (hatchPanel != null)
            {
                holdProgressBar = hatchPanel.GetComponentInChildren<Slider>();
                hatchPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("HatchInteractPanel을 찾을 수 없음!");
            }
        }
        else
        {
            Debug.LogError("UI 루트 오브젝트를 찾을 수 없음!");
        }
    }

    public void Interact(int playerID)
    {
        GameObject playerObject = FindObjectByViewID(playerID);
        if (playerObject == null) return;
        PlayerRoleDistribution prd = playerObject.GetComponent<PlayerRoleDistribution>();
        if (prd == null) return;

        if (!isHolding && prd.role == PlayerRole.Citizen)
        {
            isHolding = true;
            interactingPlayerID = playerID;

            if (hatchPanel != null) hatchPanel.SetActive(true);
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            Debug.Log($"Hold Hatch Interact time [" + holdTime + "]");

            if (holdProgressBar != null)
                holdProgressBar.value = holdTime / holdDuration;

            if (holdTime >= holdDuration)
            {
                isHolding = false;
                holdTime = 0f;

                photonView.RPC("Escape", RpcTarget.All, interactingPlayerID);

                if (hatchPanel != null) hatchPanel.SetActive(false);
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                CancelInteraction();
            }
        }
    }

    public void CancelInteraction()
    {
        isHolding = false;
        holdTime = 0f;
        interactingPlayerID = -1;

        if (hatchPanel != null) hatchPanel.SetActive(false);
    }

    [PunRPC]
    private void Escape(int playerID)
    {
        Debug.Log($"Hatch : Player {playerID} has escaped!");
        GameManager.Instance.EliminateOrEscapeCitizen(playerID, "Escape");
    }

    GameObject FindObjectByViewID(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        return pv != null ? pv.gameObject : null;
    }

    private Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform found = FindChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }
}
