using Photon.Pun;
using UnityEngine;

public class FarmingObject : MonoBehaviourPun, IInteractableObject
{
    public Item item;

    public void Interact(int playerID)
    {
        GameObject player = PhotonView.Find(playerID).gameObject;
        Inventory playerInventory = player.GetComponent<Inventory>();

        if (playerInventory.AddItem(item))
        {
            ObjectPool.instance.ReturnObject(gameObject, item.itemName);
        }
    }
}
