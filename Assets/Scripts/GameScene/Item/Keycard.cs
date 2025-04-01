using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Keycard", menuName = "ScriptableObjects/Keycard")]
public class Keycard : ItemData
{
    public RayModule rayModule;
    public float useDistance = 2f;

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
}
