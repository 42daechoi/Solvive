using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager_Game : MonoBehaviour
{
    // Player Movement
    public event Action<float, float> OnPlayerMove;
    public event Action<bool> OnPlayerSprint;
    public event Action OnPlayerJump;


    // Player Active
    public event Action OnDropItem;
    public event Action<int> OnHeldItem;
    public event Action OnUseItem;
    public event Action OnInteraction;

    // Player Animation
    public event Action<string> OnAnimationStateChanged;

    // Interaction
    public event Action<Item> OnOpenDoor;
    public event Action<bool> OnUseComputer;
    public event Action<int, Vector3, Quaternion> OnMoveToComputer;
    public event Action OnExitComputer;
    public event Action OnAllGeneratorsActivated;
    public event Action<char> OnTypeNumberAtComputer;
    public event Action OnTypeBackspaceAtComputer;

    // Game Logic
    public event Action<bool> OnCameraActive;
    public event Action<int> OnRemoveItem;



    public static EventManager_Game Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 패턴 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void InvokePlayerMove(float horizontal, float vertical)
    {
        OnPlayerMove?.Invoke(horizontal, vertical);
    }

    // 스프린트 이벤트 발행
    public void InvokeSprint(bool isSprint)
    {
        OnPlayerSprint?.Invoke(isSprint);
    }
    
    public void InvokeCameraActive(bool isActive)
    {
        OnCameraActive?.Invoke(isActive);
    }

    public void InvokePlayerJump()
    {
        OnPlayerJump?.Invoke();
    }
    public void InvokeInteraction()
    {
        OnInteraction?.Invoke();
    }
    public void InvokeHeldItem(int keyCode)
    {
        OnHeldItem?.Invoke(keyCode);
    }

    public void InvokeDropItem()
    {
        OnDropItem?.Invoke();
    }

    public void InvokeUseItem()
    {
        OnUseItem?.Invoke();
    }
    

    public void InvokeRemoveItem(int slotIndex)
    {
        OnRemoveItem?.Invoke(slotIndex);
    }
    
    public void InvokeOpenDoor(Item usedItem)
    {
        OnOpenDoor?.Invoke(usedItem);
    }

    public void InvokeUseComputer(bool isActComputer)
    {
        OnUseComputer?.Invoke(isActComputer);
    }
    
    public void InvokeMoveToComputer(int playerId, Vector3 targetPosition, Quaternion targetRotation)
    {
        OnMoveToComputer?.Invoke(playerId, targetPosition, targetRotation);
    }
    
    public void InvokeExitComputer()
    {
        Debug.Log("이벤트매니저 컴퓨터강제종료 발행");
        OnExitComputer?.Invoke();
    }

    public void InvokeAllGeneratorsActivated()
    {
        OnAllGeneratorsActivated?.Invoke();
    }
    
    public void InvokeAnimationStateChange(string animationState)
    {
        OnAnimationStateChanged?.Invoke(animationState);
    }

    public void InvokeTypeNumberAtComputer(char keyCode)
    {
        OnTypeNumberAtComputer?.Invoke(keyCode);
    }

    public void InvokeTypeBackspaceAtComputer()
    {
        OnTypeBackspaceAtComputer?.Invoke();
    }
}

