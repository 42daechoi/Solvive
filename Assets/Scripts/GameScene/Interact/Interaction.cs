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
                Debug.Log("상호 작용 오브젝트 레이캐스트 히트 성공");
                interactableObject.Interact(photonView.ViewID);
            }
            else
            {
                Debug.Log("상호 작용 오브젝트 레이캐스트 히트 실패");
            }
        }
        else
        {
            Debug.Log("레이캐스트가 아무 오브젝트도 맞추지 못했습니다.");
        }
    }

    public void RunInteraction()
    {
        TryInteraction();
    }
}
