using Photon.Pun;
using UnityEngine;

public class FarmingObject : MonoBehaviourPun, IInteractableObject
{
    public ItemData itemData;
    private int viewID;

    public void Interact(int playerID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;
        Inventory playerInventory = player.GetComponent<Inventory>();
        viewID = photonView.ViewID;

        if (playerInventory.AddItem(this))
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
        return viewID;
    }
}
