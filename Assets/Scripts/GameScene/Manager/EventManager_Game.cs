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
    public event Action<bool> OnFPSflashlightToggle;
    public event Action OnInteraction;

    // Player Animation
    public event Action<string> OnAnimationStateChanged;

    // Interaction
    public event Action<ItemData> OnOpenDoor;
    public event Action<bool> OnUseComputer;
    public event Action<int, Vector3, Quaternion> OnMoveToComputer;
    public event Action<int> OnExitComputer;
    public event Action OnAllGeneratorsActivated;
    public event Action<char, int> OnTypeNumberAtComputer;
    public event Action<int> OnTypeBackspaceAtComputer;

    // Game Logic
    public event Action<bool> OnCameraActive;
    public event Action<int> OnRemoveItem;
    public event Action OnAllPlayerSpawned;
    public event Action<int> OnChangeUnlockedComputerCount;
    public event Action OnAllComputerUnlocked;
    public event Action OnOneCitizenAlive;
    public event Action OnObserverState;
    public event Action<PlayerRole> OnEndGame;



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

    public void InvokeOpenDoor(ItemData usedItem)
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

    public void InvokeExitComputer(int viewID)
    {
        OnExitComputer?.Invoke(viewID);
    }

    public void InvokeAllGeneratorsActivated()
    {
        OnAllGeneratorsActivated?.Invoke();
    }

    public void InvokeAnimationStateChange(string animationState)
    {
        OnAnimationStateChanged?.Invoke(animationState);
    }

    public void InvokeTypeNumberAtComputer(char keyCode, int playerViewID)
    {
        OnTypeNumberAtComputer?.Invoke(keyCode, playerViewID);
    }

    public void InvokeTypeBackspaceAtComputer(int playerViewID)
    {
        OnTypeBackspaceAtComputer?.Invoke(playerViewID);
    }

    public void InvokeAllPlayerSpawned()
    {
        OnAllPlayerSpawned?.Invoke();
    }

    public void InvokeChangeUnlockedComputerCount(int count)
    {
        OnChangeUnlockedComputerCount?.Invoke(count);
    }

    public void InvokeAllComputerUnlocked()
    {
        OnAllComputerUnlocked?.Invoke();
    }

    public void InvokeObserverState()
    {
        OnObserverState?.Invoke();
    }

    public void InvokeOneCitizenAlive()
    {
        Debug.Log("EventManger_Game : 시민 혼자 남음 이벤트 발생");
        OnOneCitizenAlive?.Invoke();
    }

    public void InvokeEndGame(PlayerRole playerRole)
    {
        Debug.Log("EventManger_Game : 게임 종료 이벤트 발생 [승자:" + playerRole + "]");

        OnEndGame?.Invoke(playerRole);
    }

    public void InvokeFPSlightToggle(bool isOn)
    {
        OnFPSflashlightToggle?.Invoke(isOn);
    }
}