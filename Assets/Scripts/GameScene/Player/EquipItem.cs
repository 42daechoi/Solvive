using System.Collections;
using DG.Tweening;
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
    
    private bool isProcessing = false;
    

    public GameObject Equip(FarmingObject item)
    {
        
        if (photonView.IsMine)
        {
            if (item == null)
            {
                FirstPersonEquipProcess(null);
            }
            else
            {
                FirstPersonEquipProcess(item.GetItemData());
            }
        }
        
        if (item == null)
        {
            EventManager_Game.Instance.InvokeAnimationStateChange("Default");
            return null;
        }
        ItemData itemData = item.GetItemData();

        GameObject equipItem = ObjectPool.instance.GetObject(item.GetViewID(), Vector3.zero, Quaternion.identity);
        if (equipItem == null)
        {
            Debug.Log("EquipItem : 오브젝트 풀에서 장착할 아이템을 받아오지 못했습니다.");
            return null;
        }
        
        int viewID = equipItem.GetPhotonView().ViewID;
        if (equipItem)
        {
            photonView.RPC("SyncEquipItem", RpcTarget.All, viewID, itemData.equipPosition, itemData.equipRotation, photonView.ViewID);
            EventManager_Game.Instance.InvokeAnimationStateChange(itemData.animationState);
        }
        StartCoroutine(CheckItemIsPasswordPaper(itemData.itemName, equipItem));

        return equipItem;
    }

    private IEnumerator CheckItemIsPasswordPaper(string itemName, GameObject equipItem)
    {
        if (itemName == "PasswordPaper")
        {
            yield return new WaitForSeconds(0.2f);
            SyncPassword syncPassword = GetComponentInChildren<SyncPassword>(true);
            syncPassword.SetPasswordToFirstPersonPaper(equipItem);
        }
    }
    
    private void FirstPersonEquipProcess(ItemData item)
    {
        if (isProcessing) return;
        isProcessing = true;
        IsEquipping = true;

        UnequipArmRotation(() =>
        {
            if (item == null)
            {
                isProcessing = false;
                IsEquipping = false;
                return;
            }

            EquipFromUnequipPose(item.itemName, () =>
            {
                isProcessing = false;
                IsEquipping = false;
            });
        });
    }

    #region Equip코루틴
    private void EquipFromUnequipPose(string itemName, System.Action onComplete)
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
            return;
        }

        float duration = 0.3f;

        Sequence seq = DOTween.Sequence();

        seq.Join(r_ArmStrech.DOLocalRotateQuaternion(Quaternion.Euler(poseData.r_ArmStrech), duration));
        seq.Join(r_Forearm.DOLocalRotateQuaternion(Quaternion.Euler(poseData.r_ForearmRotation), duration));
        seq.Join(r_Hand.DOLocalRotateQuaternion(Quaternion.Euler(poseData.r_HandRotation), duration));

        seq.Join(l_ArmStrech.DOLocalRotateQuaternion(Quaternion.Euler(poseData.l_ArmStrech), duration));
        seq.Join(l_Forearm.DOLocalRotateQuaternion(Quaternion.Euler(poseData.l_ForearmRotation), duration));
        seq.Join(l_Hand.DOLocalRotateQuaternion(Quaternion.Euler(poseData.l_HandRotation), duration));

        seq.SetEase(Ease.OutSine).OnComplete(() => onComplete?.Invoke());
    }
    #endregion

    private Transform GetEquipTransform() => _equipTransform;

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
        
        // 1인칭에서는 렌더러 비활성화
        if (playerPhotonView.IsMine)
        {
            Renderer[] renderers = equipItem.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            
            if (equipItem.name.Contains("Flashlight"))
            {
                Light light = equipItem.GetComponentInChildren<Light>();
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }
    }

    public void UnEquip(GameObject itemObject, bool isReturnPool, bool needCollider)
    {
        if (itemObject)
        {
            if (isReturnPool)
                ObjectPool.instance.ReturnObject(itemObject);

            int viewID = itemObject.GetPhotonView().ViewID;
            photonView.RPC("SyncUnequip", RpcTarget.All, viewID, needCollider);
        }

        if (photonView.IsMine && currentFPSWeapon != null)
        {
            UnequipArmRotation(() =>
            {
                EventManager_Game.Instance.InvokeAnimationStateChange("Default");
            });
        }
    }

    #region Unequip코루틴 애니메이션
    private void UnequipArmRotation(System.Action onComplete)
    {
        GameObject weaponToDisable = currentFPSWeapon;

        Quaternion targetLeftArmRotation = Quaternion.Euler(25.05f, -84.53f, 63.9f);
        Quaternion targetRightArmRotation = Quaternion.Euler(-0.84f, 73.5f, -51.11f);

        float duration = 0.2f;

        Sequence seq = DOTween.Sequence();
        seq.Join(l_ArmStrech.DOLocalRotateQuaternion(targetLeftArmRotation, duration));
        seq.Join(r_ArmStrech.DOLocalRotateQuaternion(targetRightArmRotation, duration));

        seq.SetEase(Ease.OutSine).OnComplete(() =>
        {
            if (weaponToDisable != null)
            {
                weaponToDisable.SetActive(false);
                if (currentFPSWeapon == weaponToDisable)
                {
                    currentFPSWeapon = null;
                }
            }
            onComplete?.Invoke();
        });
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
                renderer.enabled = true;
        }

        if (unequipItem.name.Contains("Flashlight"))
        {
            Light light = unequipItem.GetComponentInChildren<Light>();
            if (light != null) light.enabled = false;
        }

        unequipItem.transform.SetParent(null);
        if (needCollider)
            unequipItem.GetComponent<Collider>().enabled = true;
    }
    public bool IsEquipping { get; private set; } = false;
}