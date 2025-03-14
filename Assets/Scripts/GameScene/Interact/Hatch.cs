using Photon.Pun;
using UnityEngine;

public class Hatch : MonoBehaviourPun, IInteractableObject
{
    private bool isHolding = false;
    private float holdTime = 0f;
    private float holdDuration = 5f;
    private int interactingPlayerID = -1;

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
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            Debug.Log($"��ġ ��ȣ �ۿ� ������ [" + holdTime + "]");

            if (holdTime >= holdDuration)
            {
                isHolding = false;
                holdTime = 0f;

                photonView.RPC("Escape", RpcTarget.All, interactingPlayerID);
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
    }

    [PunRPC]
    private void Escape(int playerID)
    {
        Debug.Log($"Hatch : Player {playerID} has escaped!");
        GameManager.Instance.EliminateOrEscapeCitizen(playerID);
    }

    GameObject FindObjectByViewID(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        return pv != null ? pv.gameObject : null;
    }

}
