using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotationDoor : MonoBehaviour
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
        EventManager_Game.Instance.OnOpenDoor += HandleOpenDoor;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnOpenDoor -= HandleOpenDoor;
    }
    
    public void SetFocus(bool focus)
    {
        isFocused = focus;
    }

    private void HandleOpenDoor(ItemData heldItem)
    {
        OpenBothDoors();
    }

    private void OpenBothDoors()
    {
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

        // 10초 후 CloseDoor 실행
        Invoke(nameof(CloseBothDoors), 10f);
    }

    private void CloseBothDoors()
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
    }
}
    
