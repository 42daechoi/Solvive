using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class RotationDoor : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform door1;
    [SerializeField] private Transform door2;

    // 문이 열렸는지 닫혔는지 상태를 표시할 변수
    private bool isDoorOpen = false;

    private bool isFocused = false;
    
    [SerializeField] private Animator door1Animator;
    [SerializeField] private Animator door2Animator;
    
    private void OnEnable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOpenDoor += HandleOpenDoor;
        }
        else
        {
            Debug.LogWarning("EventManager_Game.Instance is null - RotationDoor");
        }
    }

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOpenDoor -= HandleOpenDoor;
        }
        else
        {
            Debug.LogWarning("EventManager_Game.Instance is null - RotationDoor");
        }
    }
    
    public void SetFocus(bool focus)
    {
        isFocused = focus;
    }

    private void HandleOpenDoor(ItemData heldItem)
    {
        if (isDoorOpen) return;
        photonView.RPC(nameof(RPC_OpenBothDoors), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_OpenBothDoors()
    {
        if (isDoorOpen) return;
        Debug.Log("문 열기");

        if (door1Animator != null) 
        {
            door1Animator.ResetTrigger("CloseDoor1"); // 기존 트리거 초기화
            door1Animator.SetTrigger("OpenDoor1"); // OpenDoor1 실행
        }

        if (door2Animator != null) 
        {
            door2Animator.ResetTrigger("CloseDoor2");
            door2Animator.SetTrigger("OpenDoor2"); // OpenDoor2 실행
        }
        isDoorOpen = true;
        
        photonView.RPC(nameof(RPC_StartCloseCountdown), RpcTarget.All);
    }
    
    [PunRPC]
    private void RPC_StartCloseCountdown()
    {
        StartCoroutine(DelayedClose());
    }
    
    private IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(10f);
        photonView.RPC(nameof(RPC_CloseBothDoors), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void RPC_CloseBothDoors()
    {
        Debug.Log("문 닫기");

        if (door1Animator != null) 
        {
            door1Animator.ResetTrigger("OpenDoor1");
            door1Animator.SetTrigger("CloseDoor1"); // CloseDoor1 실행
        }

        if (door2Animator != null) 
        {
            door2Animator.ResetTrigger("OpenDoor2");
            door2Animator.SetTrigger("CloseDoor2"); // CloseDoor2 실행
        }
        isDoorOpen = false;
    }
}
    
