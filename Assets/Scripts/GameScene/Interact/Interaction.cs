using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Interaction : MonoBehaviourPun
{
    public float interactionRange = 3.0f;
    
    private void TryInteraction()
    {
        // 화면 중심에서 발사되는 레이 생성
        if (!photonView.IsMine) return;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        int layerMask = ~LayerMask.GetMask("Player", "Hitbox");
        
        Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red, 1.0f);
        
        if (Physics.Raycast(ray, out hit, interactionRange, layerMask))
        {
            IInteractableObject interactableObject = hit.collider.GetComponent<IInteractableObject>();
            if (interactableObject != null)
            {
                interactableObject.Interact(photonView.ViewID);
            }
            else
            {
            }
        }
        else
        {
        }
    }

    public void RunInteraction()
    {
        TryInteraction();
    }
}
