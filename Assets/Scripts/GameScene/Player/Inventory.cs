using System;
using Photon.Pun;
using UnityEngine;

public class Inventory : MonoBehaviourPun
{
    [SerializeField]private Item[] itemSlots = new Item[4];

    private void OnEnable()
    {
        EventManager_Game.Instance.OnRemoveItem += RemoveItem;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnRemoveItem -= RemoveItem;
    }

    public bool AddItem(Item item)
    {
        if (!photonView.IsMine) return false;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                Debug.Log($"{item.itemName}을 획득하였습니다.");
                int playerID = photonView.ViewID;
                photonView.RPC("SyncInventory", RpcTarget.All, playerID, i, item.itemName);
                InventoryUI.Instance.UpdateUI(this);
                return true;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    [PunRPC]
    private void SyncInventory(int playerID, int idx, string itemName)
    {
        try
        {
            PhotonView playerPV = PhotonView.Find(playerID);
            GameObject playerObj = playerPV.gameObject;

            if (playerPV.TryGetComponent(out Inventory inventory))
            {
                if (itemName == null)
                {
                    inventory.SetItem(idx, null);
                    return;
                }
                inventory.SetItem(idx, ItemManager.Instance.GetItemByName(itemName));
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"Inventory : {e.Message}");
        }

    }

    public void RemoveItem(int slotIndex)
    {
        if (!photonView.IsMine) return;
        if (itemSlots[slotIndex] != null)
        {
            int playerID = photonView.ViewID;
            photonView.RPC("SyncInventory", RpcTarget.All, playerID, slotIndex, null);
            InventoryUI.Instance.UpdateUI(this);
        }
        else
        {
            Debug.Log("비어있는 슬롯으로 버리기를 시도할 수 없습니다.");
        }
    }

    public Item GetItem(int slotIndex)
    {
        return itemSlots[slotIndex];
    }

    public void SetItem(int slotIdx, Item item)
    {
        itemSlots[slotIdx] = item;
    }

    public Item[] GetItemSlots()
    {
        return itemSlots;
    }
}
