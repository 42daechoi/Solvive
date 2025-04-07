using Photon.Pun;
using UnityEngine;

public class FarmingObject : MonoBehaviourPun, IInteractableObject
{
    public ItemData itemData;
    private bool isPickedUp = false;

    public void Interact(int playerID)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RequestPickup), RpcTarget.MasterClient, playerID);
        }
        else
        {
            HandlePickup(playerID);
        }
    }

    [PunRPC]
    private void RequestPickup(int playerID)
    {
        HandlePickup(playerID);
    }

    private void HandlePickup(int playerID)
    {
        if (isPickedUp) return;

        isPickedUp = true;

        photonView.RPC(nameof(ReceivePickup), RpcTarget.All, playerID);
    }

    public void SetIsPickUp(bool flag)
    {
        photonView.RPC(nameof(SyncToMasterIsPickUp), RpcTarget.MasterClient, flag);
    }

    [PunRPC]
    private void SyncToMasterIsPickUp(bool flag)
    {
        isPickedUp = flag;
    }

    [PunRPC]
    private void ReceivePickup(int playerID)
    {
        PhotonView playerPV = PhotonView.Find(playerID);
        GameObject player = playerPV.gameObject;
        Inventory inventory = player.GetComponent<Inventory>();

        if (inventory.AddItem(this))
        {
            ObjectPool.instance.ReturnObject(gameObject);
        }
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public int GetViewID()
    {
        return photonView.ViewID;
    }
}
