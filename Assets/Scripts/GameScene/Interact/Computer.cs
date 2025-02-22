using System;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class Computer : MonoBehaviourPun, IInteractableObject
{
    //[SerializeField] private CinemachineVirtualCamera moniterCamera;
    [SerializeField] private Canvas moniterCanvas;
    [SerializeField] private Transform interactionPoint;

    private int usingPlayerID;
    private bool IsAllGeneratorsActivated;
    private bool OnInteraction;

    private void OnEnable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnAllGeneratorsActivated += HandleAllGeneratorsActivated;
            EventManager_Game.Instance.OnExitComputer += ForceExit;
        }
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnAllGeneratorsActivated -= HandleAllGeneratorsActivated;
        EventManager_Game.Instance.OnExitComputer -= ForceExit;
    }

    void Start()
    {
        EventManager_Game.Instance.OnAllGeneratorsActivated += HandleAllGeneratorsActivated;
        EventManager_Game.Instance.OnExitComputer += ForceExit;
        IsAllGeneratorsActivated = false;
        OnInteraction = false;
        usingPlayerID = -1;
    }

    public void Interact(int playerId)
    {
        Debug.Log($"Computer : {IsAllGeneratorsActivated}");
        if (IsAllGeneratorsActivated == false)
        {
            return;
        }
        if (!OnInteraction)
        {
            OnInteraction = true;
        }

        Vector3 worldPosition = transform.TransformPoint(interactionPoint.localPosition);
        Quaternion worldRotation = interactionPoint.rotation;
        photonView.RPC("SetUsingPlayerID", RpcTarget.All, playerId);
        EventManager_Game.Instance.InvokeMoveToComputer(playerId, worldPosition, worldRotation);
        EventManager_Game.Instance.InvokeUseComputer(OnInteraction);
    }

    private void ForceExit()
    {
        Debug.Log("컴퓨터 강제 종료");

        OnInteraction = false;
        usingPlayerID = -1;
        if (EventManager_Game.Instance != null)
        {
            Debug.Log("이벤트 매니저 호출 성공");
            EventManager_Game.Instance.InvokeUseComputer(OnInteraction);
        }
    }

    [PunRPC]
    private void SetUsingPlayerID(int playerID)
    {
        usingPlayerID = playerID;
    }

    private void HandleAllGeneratorsActivated()
    {
        IsAllGeneratorsActivated = true;
        moniterCanvas.gameObject.SetActive(true);
        Debug.Log("컴퓨터 상호작용 활성화.");
    }

    public int GetUsingPlayerID()
    {
        return usingPlayerID;
    }
}
