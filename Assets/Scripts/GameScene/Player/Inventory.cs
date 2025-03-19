using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Inventory : MonoBehaviourPunCallbacks
{
    [SerializeField] private FarmingObject[] itemSlots = new FarmingObject[4];

    public override void OnEnable()
    {
        EventManager_Game.Instance.OnRemoveItem += RemoveItem;
        EventManager_Game.Instance.OnEliminateOrEscape += DropAllItems;
    }

    public override void OnDisable()
    {
        EventManager_Game.Instance.OnRemoveItem -= RemoveItem;
        EventManager_Game.Instance.OnEliminateOrEscape -= DropAllItems;
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

                //AddProperties(itemViewID);
                photonView.RPC("SyncInventory", RpcTarget.All, photonView.ViewID, i, itemViewID);
                InventoryUI.Instance.UpdateUI(this);
                return true;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    //private void AddProperties(int itemViewID)
    //{
    //    ExitGames.Client.Photon.Hashtable playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

    //    if (!playerProps.ContainsKey("Inventory"))
    //    {
    //        playerProps["Inventory"] = new int[0];
    //    }

    //    int[] inventoryArray = (int[])playerProps["Inventory"];

    //    List<int> inventoryList = new List<int>(inventoryArray);
    //    inventoryList.Add(itemViewID);
    //    playerProps["Inventory"] = inventoryList.ToArray();


    //    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    //}

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
            //RemoveProperties(itemSlots[slotIndex].GetViewID());
            photonView.RPC("SyncInventory", RpcTarget.All, playerID, slotIndex, 0);
            itemSlots[slotIndex] = null;
            InventoryUI.Instance.UpdateUI(this);
        }
        else
        {
            Debug.Log("비어있는 슬롯으로 버리기를 시도할 수 없습니다.");
        }
    }

    //private void RemoveProperties(int itemViewID)
    //{
    //    ExitGames.Client.Photon.Hashtable playerProps = PhotonNetwork.LocalPlayer.CustomProperties;

    //    if (!playerProps.ContainsKey("Inventory"))
    //    {
    //        return;
    //    }

    //    int[] inventoryArray = (int[])playerProps["Inventory"];
    //    List<int> inventoryList = new List<int>(inventoryArray);

    //    if (inventoryList.Contains(itemViewID))
    //    {
    //        inventoryList.Remove(itemViewID);
    //    }
    //    playerProps["Inventory"] = inventoryList.ToArray();
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    //}



    private void DropAllItems(string flag)
    {
        if (!photonView.IsMine) return;

        if (flag == "Eliminate")
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] != null)
                {
                    Vector3 dropPosition = GetRandomDropPosition(transform.position);
                    GameObject dropItem = ObjectPool.instance.GetObject(itemSlots[i].GetViewID(), dropPosition, Quaternion.identity);
                    photonView.RPC("SyncInventory", RpcTarget.All, photonView.ViewID, i, 0);
                    itemSlots[i] = null;
                }
            }
            InventoryUI.Instance.UpdateUI(this);
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

    //public override void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    if (!PhotonNetwork.IsMasterClient) return;
    //    Vector3 playerPosition = new Vector3(-29.77029f, 20.269f, 21.33452f);
    //    if (otherPlayer.CustomProperties.TryGetValue("LastPosition", out object posData))
    //    {
    //        float[] posArray = (float[])posData;
    //        if (posArray != null)
    //        {
    //            playerPosition = new Vector3(posArray[0], posArray[1], posArray[2]);
    //            Debug.Log("Inventory : " + playerPosition);
    //        }
    //    }
    //    if (otherPlayer.CustomProperties.TryGetValue("Inventory", out object inventoryData))
    //    {
    //        List<int> inventory = inventoryData as List<int>;
    //        if (inventory != null)
    //        {
    //            foreach (int itemID in inventory)
    //            {

    //                Vector3 dropPosition = GetRandomDropPosition(playerPosition);
    //                GameObject dropItem = ObjectPool.instance.GetObject(itemID, dropPosition, Quaternion.identity);
    //                Debug.Log("Inventory : 아이템 아이디" + itemID);
    //            }
    //        }
    //    }
    //}

    private Vector3 GetRandomDropPosition(Vector3 TargetPosition)
    {
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
        float randomDistance = UnityEngine.Random.Range(0.5f, 2f);
        Vector3 dropPosition = TargetPosition + randomDirection * randomDistance;
        dropPosition.y = TargetPosition.y;

        return dropPosition;
    }
}
