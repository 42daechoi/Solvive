using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Keycard", menuName = "ScriptableObjects/Keycard")]
public class Keycard : ItemData
{
    public RayModule rayModule;
    public float useDistance = 2f;
    public DestoryKeyCard destoryKeyCard;

    public override void UseItem()
    {
        RaycastHit? raycastHit = rayModule.ExecuteRayAction(GetShooterTransform(), 2);
        
        if (raycastHit.HasValue)
        {
            GameObject hitObj = raycastHit.Value.collider.gameObject;

            if (hitObj.CompareTag("Button"))
            {
                string buttonName = hitObj.name;
                Debug.Log("Button pressed: " + buttonName);

                EventManager_Game.Instance.InvokeFPSUseItem(itemName);
                EventManager_Game.Instance.InvokeOpenDoor(this, buttonName);
                TryDestroyKeycard();
            }
        }
    }
    
    private Transform GetShooterTransform()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            return playerObj.transform;
        return null;
    }
    
    private void TryDestroyKeycard()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            HeldItem holder = playerObj.GetComponent<HeldItem>();
            if (holder != null && holder.GetItem() != null)
            {
                DestoryKeyCard destroyScript = holder.GetItem().GetComponent<DestoryKeyCard>();
                if (destroyScript != null)
                {
                    destroyScript.RequestDestroy();
                }
                else
                {
                    Debug.LogWarning("DestroyKeyCard 스크립트가 카드 오브젝트에 없습니다.");
                }
            }
        }
    }
}