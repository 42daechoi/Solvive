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
            Debug.Log($"해치 상호 작용 진행중 [" + holdTime + "]");

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
        Debug.Log($"Player {playerID} has escaped!");
        // 탈출 처리 (씬 이동, UI 업데이트 등)
    }

    GameObject FindObjectByViewID(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        return pv != null ? pv.gameObject : null;
    }

}
