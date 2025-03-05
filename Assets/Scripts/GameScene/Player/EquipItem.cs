using UnityEngine;
using Photon.Pun;

public class EquipItem : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform _equipTransform;
    
    //1인칭 무기전용 오브젝트들임
    //오른손
    [SerializeField] private Transform r_ArmStrech;
    [SerializeField] private Transform r_Forearm;
    [SerializeField] private Transform r_Hand;
    [SerializeField] private Transform l_ArmStrech;
    [SerializeField] private Transform l_Forearm;
    [SerializeField] private Transform l_Hand;
    
    [SerializeField] private WeaponPoseDatabase weaponPoseDB;
    [SerializeField] private GameObject[] firstPersonWeapons;
    private GameObject currentFPSWeapon;

    public GameObject Equip(Item item)
    {
        if (item == null)
        {
            return null;
        }
        GameObject equipItem = ObjectPool.instance.GetObject(item.itemName, Vector3.zero, Quaternion.identity);
        if (equipItem == null)
        {
            Debug.Log("EquipItem : 오브젝트 풀에서 장착할 아이템을 받아오지 못했습니다.");
            return null;
        }
        int viewID = equipItem.GetPhotonView().ViewID;
        if (equipItem)
        {
            photonView.RPC("SyncEquipItem", RpcTarget.All, viewID, item.equipPosition, item.equipRotation, photonView.ViewID);
            string animationState = item.itemName == "Battery" ? "Carry" : "Default";
            EventManager_Game.Instance.InvokeAnimationStateChange(animationState);
        }
        if (photonView.IsMine)
        {
            ActivateFPSWeapon(item.itemName);
        }
        return equipItem;
    }

    private void ActivateFPSWeapon(string itemName)
    {
        foreach (GameObject weapon in firstPersonWeapons)
        {
            weapon.SetActive(false);
        }

        GameObject firstPersonWeapon = System.Array.Find(firstPersonWeapons, w => w.name == itemName);
        if (firstPersonWeapon != null)
        {
            firstPersonWeapon.SetActive(true);
            currentFPSWeapon = firstPersonWeapon;
            
            ActivateFirstPersonWeapon(itemName);
        }
    }
    
    private void ActivateFirstPersonWeapon(string itemName)
    {
        WeaponPoseData poseData = System.Array.Find(weaponPoseDB.weaponPoses, w => w.weaponName == itemName);
        if (poseData != null)
        {
            r_ArmStrech.localRotation = Quaternion.Euler(poseData.r_ArmStrech);
            r_Forearm.localRotation = Quaternion.Euler(poseData.r_ForearmRotation);
            r_Hand.localRotation = Quaternion.Euler(poseData.r_HandRotation);
            l_ArmStrech.localRotation = Quaternion.Euler(poseData.l_ArmStrech);
            l_Forearm.localRotation = Quaternion.Euler(poseData.l_ForearmRotation);
            l_Hand.localRotation = Quaternion.Euler(poseData.l_HandRotation);
        }
        else
        {
            Debug.LogWarning($"EquipItem : WeaponPoseData에 {itemName} 존재x");
        }
        
    }

    private Transform GetEquipTransform()
    {
        return _equipTransform;
    }

    [PunRPC]
    private void SyncEquipItem(int viewID, Vector3 equipPosition, Vector3 equipRotation, int playerViewID)
    {
        GameObject equipItem = PhotonView.Find(viewID).gameObject;

        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        if (playerPhotonView == null) return;

        EquipItem localEquipItem = playerPhotonView.GetComponent<EquipItem>();
        if (localEquipItem == null) return;

        if (playerPhotonView.IsMine)
        {
            equipItem.SetActive(false);
            return;
        }
        equipItem.transform.SetParent(localEquipItem._equipTransform);
        equipItem.GetComponent<Collider>().enabled = false;
        equipItem.transform.localPosition = equipPosition;
        equipItem.transform.localRotation = Quaternion.Euler(equipRotation);
    }

    public void UnEquip(Item item, GameObject itemObject, bool isReturnPool, bool needCollider)
    {
        if (itemObject)
        {
            if (isReturnPool)
            {
                ObjectPool.instance.ReturnObject(itemObject, item.itemName);
            }
            int viewID = itemObject.GetPhotonView().ViewID;
            photonView.RPC("SyncUnequip", RpcTarget.All, viewID, needCollider);
        }

        if (photonView.IsMine && currentFPSWeapon != null)
        {
            currentFPSWeapon.SetActive(false);
            currentFPSWeapon = null;
            
            r_ArmStrech.localRotation = Quaternion.identity;
            r_Forearm.localRotation = Quaternion.identity;
            r_Hand.localRotation = Quaternion.identity;
            
            l_ArmStrech.localRotation = Quaternion.identity;
            l_Forearm.localRotation = Quaternion.identity;
            l_Hand.localRotation = Quaternion.identity;
        }
    }

    [PunRPC]
    private void SyncUnequip(int viewID, bool needCollider)
    {
        GameObject unequipItem = PhotonView.Find(viewID).gameObject;
        if (photonView.IsMine)
        {
            unequipItem.SetActive(true);
        }
        unequipItem.transform.SetParent(null);
        if (needCollider)
        {
            unequipItem.GetComponent<Collider>().enabled = true;
        }
    }
}
