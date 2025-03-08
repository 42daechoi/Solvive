using System;
using Photon.Pun;
using UnityEngine;

public class Inventory : MonoBehaviourPun
{
    [SerializeField] private FarmingObject[] itemSlots = new FarmingObject[4];

    private void OnEnable()
    {
        EventManager_Game.Instance.OnRemoveItem += RemoveItem;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnRemoveItem -= RemoveItem;
    }

    public bool AddItem(FarmingObject farmingObject)
    {
        if (!photonView.IsMine) return false;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                ItemData itemData = farmingObject.GetItemData();
                int itemViewID = farmingObject.GetViewID();

                Debug.Log($"Inventory : {itemData.itemName}을 획득하였습니다.");
                itemSlots[i] = farmingObject;

                photonView.RPC("SyncInventory", RpcTarget.All, photonView.ViewID, i, itemViewID);
                InventoryUI.Instance.UpdateUI(this);
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    [PunRPC]
    private void SyncInventory(int playerID, int idx, int farmingObjectViewID)
    {
        try
        {
            PhotonView playerPV = PhotonView.Find(playerID);
            GameObject playerObj = playerPV.gameObject;

            if (playerPV.TryGetComponent(out Inventory inventory))
            {
                if (farmingObjectViewID == 0)
                {
                    inventory.SetItem(idx, null);
                    return;
                }

                PhotonView farmingObjectPV = PhotonView.Find(farmingObjectViewID);
                if (farmingObjectPV != null && farmingObjectPV.TryGetComponent(out FarmingObject farmingObject))
                {
                    inventory.SetItem(idx, farmingObject);
                }
                else
                {
                    Debug.LogError($"FarmingObject not found with ViewID {farmingObjectViewID}");
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"Inventory : {e.Message}");
        }
    }


    public void SetItem(int idx, FarmingObject fo)
    {
        itemSlots[idx] = fo;
    }

    public void RemoveItem(int slotIndex)
    {
        if (!photonView.IsMine) return;

        if (itemSlots[slotIndex] != null)
        {
            int playerID = photonView.ViewID;
            photonView.RPC("SyncInventory", RpcTarget.All, playerID, slotIndex, 0);
            itemSlots[slotIndex] = null;
            InventoryUI.Instance.UpdateUI(this);
        }
        else
        {
            Debug.Log("비어있는 슬롯으로 버리기를 시도할 수 없습니다.");
        }
    }

    public FarmingObject GetItem(int slotIndex)
    {
        return itemSlots[slotIndex];
    }

    public FarmingObject[] GetItemSlots()
    {
        return itemSlots;
    }
}
