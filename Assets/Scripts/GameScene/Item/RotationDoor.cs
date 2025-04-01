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
    [SerializeField] private Transform door3;
    [SerializeField] private Transform door4;
    [SerializeField] private Transform door5;
    [SerializeField] private Transform door6;
    [SerializeField] private Animator door1Animator;
    [SerializeField] private Animator door2Animator;
    [SerializeField] private Animator door3Animator;
    [SerializeField] private Animator door4Animator;
    [SerializeField] private Animator door5Animator;
    [SerializeField] private Animator door6Animator;
    
    private bool isDoorOpen = false;
    
    private void OnEnable()
    {
        StartCoroutine(DelayedSubscribe());
    }
    
    private IEnumerator DelayedSubscribe()
    {
        // 1-2프레임 기다리거나 짧은 시간 대기
        yield return new WaitForSeconds(0.1f);
        // 또는 yield return null; (한 프레임 대기)
    
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOpenDoor += HandleOpenDoor;
            Debug.Log("Successfully subscribed to OnOpenDoor event");
        }
        else
        {
            Debug.LogWarning("Failed to subscribe: EventManager_Game.Instance is still null after delay");
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

    private void HandleOpenDoor(ItemData heldItem, string buttonName)
    {
        if (isDoorOpen) return;
        if (PhotonNetwork.IsMasterClient)
        {
            RPC_OpenBothDoors(buttonName); 
        }
        photonView.RPC(nameof(RPC_OpenBothDoors), RpcTarget.AllViaServer, buttonName);
    }

    [PunRPC]
    private void RPC_OpenBothDoors(string buttonName)
    {
        if (isDoorOpen) return;
        switch (buttonName)
        {
            case "Button1":
                door1Animator?.ResetTrigger("CloseDoor1");
                door1Animator?.SetTrigger("OpenDoor1");

                door2Animator?.ResetTrigger("CloseDoor2");
                door2Animator?.SetTrigger("OpenDoor2");
                break;

            case "Button2":
                door3Animator?.ResetTrigger("CloseDoor3");
                door3Animator?.SetTrigger("OpenDoor3");

                door4Animator?.ResetTrigger("CloseDoor4");
                door4Animator?.SetTrigger("OpenDoor4");
                break;

            case "Button3":
                door5Animator?.ResetTrigger("CloseDoor5");
                door5Animator?.SetTrigger("OpenDoor5");

                door6Animator?.ResetTrigger("CloseDoor6");
                door6Animator?.SetTrigger("OpenDoor6");
                break;

            default:
                Debug.LogWarning($"Unknown button name: {buttonName}");
                return;
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
        photonView.RPC(nameof(RPC_CloseBothDoors), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_CloseBothDoors()
    {
        Debug.Log("문 닫기");

        if (door1Animator != null) 
        {
            door1Animator.ResetTrigger("OpenDoor1");
            door1Animator.SetTrigger("CloseDoor1");
        }

        if (door2Animator != null) 
        {
            door2Animator.ResetTrigger("OpenDoor2");
            door2Animator.SetTrigger("CloseDoor2");
        }
        
        if (door3Animator != null) 
        {
            door3Animator.ResetTrigger("OpenDoor3");
            door3Animator.SetTrigger("CloseDoor3");
        }

        if (door4Animator != null) 
        {
            door4Animator.ResetTrigger("OpenDoor4");
            door4Animator.SetTrigger("CloseDoor4");
        }
        
        if (door5Animator != null) 
        {
            door5Animator.ResetTrigger("OpenDoor5");
            door5Animator.SetTrigger("CloseDoor5");
        }

        if (door6Animator != null) 
        {
            door6Animator.ResetTrigger("OpenDoor6");
            door6Animator.SetTrigger("CloseDoor6");
        }
        isDoorOpen = false;
    }
}
    
