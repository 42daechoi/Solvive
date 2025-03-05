using System.Collections;
using UnityEngine;
using Photon.Pun;

public class EquipItem : MonoBehaviourPunCallbacks
{

    [SerializeField] private Transform _equipTransform;
    
    //1인칭 무기전용 오브젝트들임
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
            StartCoroutine(EquipFromUnequipPose(item.itemName));
        }
        return equipItem;
    }

    #region Equip코루틴
    private IEnumerator EquipFromUnequipPose(string itemName)
    {
        Quaternion startLeftArmRotation = Quaternion.Euler(25.05f, -84.53f, 63.9f);
        Quaternion startRightArmRotation = Quaternion.Euler(-8.2f, 48.5f, -35.3f);
        
        r_ArmStrech.localRotation = startRightArmRotation;
        l_ArmStrech.localRotation = startLeftArmRotation;
    
        GameObject firstPersonWeapon = System.Array.Find(firstPersonWeapons, w => w.name == itemName);
        if (firstPersonWeapon != null)
        {
            firstPersonWeapon.SetActive(true);
            currentFPSWeapon = firstPersonWeapon;
        }
        
        WeaponPoseData poseData = System.Array.Find(weaponPoseDB.weaponPoses, w => w.weaponName == itemName);
        if (poseData == null)
        {
            Debug.LogWarning($"EquipItem : WeaponPoseData에 {itemName} 존재x");
            yield break;
        }
        
        Quaternion targetRightArmRotation = Quaternion.Euler(poseData.r_ArmStrech);
        Quaternion targetRightForearmRotation = Quaternion.Euler(poseData.r_ForearmRotation);
        Quaternion targetRightHandRotation = Quaternion.Euler(poseData.r_HandRotation);
    
        Quaternion targetLeftArmRotation = Quaternion.Euler(poseData.l_ArmStrech);
        Quaternion targetLeftForearmRotation = Quaternion.Euler(poseData.l_ForearmRotation);
        Quaternion targetLeftHandRotation = Quaternion.Euler(poseData.l_HandRotation);
        
        float duration = 0.3f;
        float elapsedTime = 0f;
    
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float smoothT = t * t * (3f - 2f * t);
            
            r_ArmStrech.localRotation = Quaternion.Slerp(startRightArmRotation, targetRightArmRotation, smoothT);
            r_Forearm.localRotation = Quaternion.Slerp(Quaternion.identity, targetRightForearmRotation, smoothT);
            r_Hand.localRotation = Quaternion.Slerp(Quaternion.identity, targetRightHandRotation, smoothT);
        
            l_ArmStrech.localRotation = Quaternion.Slerp(startLeftArmRotation, targetLeftArmRotation, smoothT);
            l_Forearm.localRotation = Quaternion.Slerp(Quaternion.identity, targetLeftForearmRotation, smoothT);
            l_Hand.localRotation = Quaternion.Slerp(Quaternion.identity, targetLeftHandRotation, smoothT);
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

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
        
        equipItem.transform.SetParent(localEquipItem._equipTransform);
        equipItem.GetComponent<Collider>().enabled = false;
        equipItem.transform.localPosition = equipPosition;
        equipItem.transform.localRotation = Quaternion.Euler(equipRotation);
        
        if (playerPhotonView.IsMine)
        {
            Renderer[] renderers = equipItem.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    public void UnEquip(Item item, GameObject itemObject, bool isReturnPool, bool needCollider, bool isSwapping = false)
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
            if (isSwapping)
            {
                currentFPSWeapon.SetActive(false);
                currentFPSWeapon = null;
            }
            else
            {
                StartCoroutine(UnequipArmRotation());
            }
        }
    }

    #region Unequip코루틴 애니메이션
    private IEnumerator UnequipArmRotation()
    {
        Quaternion startRightArmRotation = r_ArmStrech.localRotation;
        Quaternion startLeftArmRotation = l_ArmStrech.localRotation;
        
        Quaternion targetLeftArmRotation = Quaternion.Euler(25.05f, -84.53f, 63.9f);
        Quaternion targetRightArmRotation = Quaternion.Euler(-8.2f, 48.5f, -35.3f);
        
        float duration = 0.4f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            // 시간에 따른 보간 계수 계산 (0에서 1 사이)
            float t = elapsedTime / duration;
        
            // 부드러운 보간을 위해 smoothstep 사용
            float smoothT = t * t * (3f - 2f * t);
        
            // 왼쪽 팔과 오른쪽 팔의 회전값 보간
            l_ArmStrech.localRotation = Quaternion.Slerp(startLeftArmRotation, targetLeftArmRotation, smoothT);
            r_ArmStrech.localRotation = Quaternion.Slerp(startRightArmRotation, targetRightArmRotation, smoothT);
        
            // 시간 업데이트
            elapsedTime += Time.deltaTime;
        
            yield return null; // 다음 프레임까지 대기
        }
    
        currentFPSWeapon.SetActive(false);
        currentFPSWeapon = null;
    }
    #endregion

    [PunRPC]
    private void SyncUnequip(int viewID, bool needCollider)
    {
        GameObject unequipItem = PhotonView.Find(viewID).gameObject;
        
        if (photonView.IsMine)
        {
            Renderer[] renderers = unequipItem.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
        
        unequipItem.transform.SetParent(null);
        if (needCollider)
        {
            unequipItem.GetComponent<Collider>().enabled = true;
        }
    }
}
