using System;
using Photon.Pun;
using UnityEngine;

public class HeldItem : MonoBehaviourPunCallbacks
{
    [SerializeField] private FarmingObject item;
    [SerializeField] private GameObject itemObject;
    [SerializeField] private int slotIndex;
    [SerializeField] private EquipItem equipItem;
    private SpawnPointManager spawnPointManager = new SpawnPointManager();
    private float dropOffset = 1f;
    public SlotHighlight slotHighlight;
    public Transform[] originalPrefabs;



    public override void OnEnable()
    {
        EventManager_Game.Instance.OnHeldItem += SelectItem;
        EventManager_Game.Instance.OnDropItem += DropItem;
        EventManager_Game.Instance.OnUseItem += UseItem;
    }

    public override void OnDisable()
    {
        EventManager_Game.Instance.OnHeldItem -= SelectItem;
        EventManager_Game.Instance.OnDropItem -= DropItem;
        EventManager_Game.Instance.OnUseItem -= UseItem;
    }

    private void Start()
    {
        GameObject UI = GameObject.Find("UI");
        slotHighlight = UI.GetComponent<SlotHighlight>();
        slotHighlight.UpdateSlotHighlight(0);
        if (TryGetComponent(out EquipItem _equipItem))
        {
            equipItem = _equipItem;
        }
        else
        {
            Debug.Log("HeldItem : EquipItem을 가져오지 못함.");
        }
        photonView.RPC("InitItemInfo", RpcTarget.All, photonView.ViewID);
    }

    private void SelectItem(int keyCode)
    {
        if (!photonView.IsMine) return;
        if (TryGetComponent(out Inventory inventory))
        {
            if (keyCode == 1)
            {
                equipItem.UnEquip(itemObject, true, true, false);
                photonView.RPC("InitItemInfo", RpcTarget.All, photonView.ViewID);
                if (slotHighlight != null)
                {
                    slotIndex = keyCode - 2;
                    slotHighlight.UpdateSlotHighlight(slotIndex + 1);
                }
                if (Raticle.Instance != null)
                {
                    Raticle.Instance.UpdateCrosshairByItemDelayed(null);
                }
            }
            else
            {
                int newSlotIndex = keyCode - 2;
                
                if (newSlotIndex == slotIndex && item != null)
                {
                    return;
                }
                
                if (item != null)
                {
                    equipItem.UnEquip(itemObject, true, true, true);
                }
                slotIndex = keyCode - 2;
                if (slotHighlight != null)
                {
                    slotHighlight.UpdateSlotHighlight(slotIndex + 1);
                }
                item = inventory.GetItem(slotIndex);
                itemObject = equipItem.Equip(item);
                if (item == null)
                {
                    Debug.Log("HeldItem : 해당 슬롯에는 아이템이 없습니다.");
                    photonView.RPC("InitItemInfo", RpcTarget.All, photonView.ViewID);
                    if (Raticle.Instance != null)
                    {
                        Raticle.Instance.UpdateCrosshairByItemDelayed(null);
                    }
                    return;
                }
                else
                {
                    photonView.RPC("SyncItemInfo", RpcTarget.Others, photonView.ViewID, item.GetViewID(), keyCode);
                }
                if (Raticle.Instance != null)
                {
                    Raticle.Instance.UpdateCrosshairByItemDelayed(item.GetItemData());
                }
            }
        }
    }

    [PunRPC]
    private void SyncItemInfo(int playerViewID, int itemViewID, int keyCode)
    {
        try
        {
            PhotonView playerPV = PhotonView.Find(playerViewID);
            GameObject playerObj = playerPV.gameObject;

            if (playerPV.TryGetComponent(out HeldItem heldItem))
            {
                if (playerPV.TryGetComponent(out Inventory inventory))
                {
                    heldItem.slotIndex = keyCode - 2;
                    heldItem.item = inventory.GetItem(slotIndex);
                    heldItem.itemObject = PhotonNetwork.GetPhotonView(itemViewID).gameObject;
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"HeldItem : {e.Message}");
        }

    }

    [PunRPC]
    private void InitItemInfo(int playerViewID)
    {
        try
        {
            PhotonView playerPV = PhotonView.Find(playerViewID);
            GameObject playerObj = playerPV.gameObject;

            if (playerPV.TryGetComponent(out HeldItem heldItem))
            {
                heldItem.item = null;
                heldItem.itemObject = null;
                heldItem.slotIndex = -10;
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"HeldItem : {e.Message}");
        }
    }

    public void ReplaceItem(Vector3 replacePosition, bool needCollider)
    {
        if (!photonView.IsMine) return;
        if (item != null)
        {
            equipItem.UnEquip(itemObject, false, needCollider);

            int viewID = itemObject.GetPhotonView().ViewID;
            item.SetIsPickUp(false);
            photonView.RPC("SyncReplaceItem", RpcTarget.All, replacePosition, viewID);
            EventManager_Game.Instance.InvokeRemoveItem(slotIndex);
            photonView.RPC("InitItemInfo", RpcTarget.All, photonView.ViewID);
        }
    }

    [PunRPC]
    private void SyncReplaceItem(Vector3 replacePosition, int itemViewID)
    {
        try
        {
            PhotonView itemPhotonView = PhotonView.Find(itemViewID);
            GameObject itemObj = itemPhotonView.gameObject;
            string itemNameWithoutClone = itemObj.name.Replace("(Clone)", "").Trim();


            itemObj.transform.position = replacePosition;
            itemObj.transform.rotation = Quaternion.identity;
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"HeldItem : {e.Message}");
        }

    }

    public void DropItem()
    {
        if (!photonView.IsMine) return;
        
        ReplaceItem(spawnPointManager.GetGroundPosition(transform.position), true);
        if (item == null) Raticle.Instance.UpdateCrosshairByItemDelayed(null);
        else Raticle.Instance.UpdateCrosshairByItemDelayed(item.GetItemData());
    }

    public FarmingObject GetItem()
    {
        return item;
    }

    public GameObject GetItemObject()
    {
        return itemObject;
    }


    public void UseItem()
    {
        if (!photonView.IsMine) return;
        if (item == null)
        {
            Debug.Log("HeldItem : 사용할 아이템이 없습니다.");
            return;
        }
        if (equipItem.IsEquipping) return;
        ItemData itemData = item.GetItemData();
        Debug.Log($"HeldItem : {item.GetItemData().itemName} 아이템 사용");
        itemData.UseItem();
        
    }

}
