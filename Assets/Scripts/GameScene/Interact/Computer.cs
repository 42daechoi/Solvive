using Photon.Pun;
using UnityEngine;

public class Computer : MonoBehaviourPun, IInteractableObject
{
    [SerializeField] private Canvas moniterCanvas;
    [SerializeField] private Transform interactionPoint;

    [SerializeField] private int usingPlayerID;
    [SerializeField] private bool onInteraction;
    private bool isAllGeneratorsActivated;

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnAllGeneratorsActivated -= HandleAllGeneratorsActivated;
            EventManager_Game.Instance.OnExitComputer -= ForceExit;
        }
    }

    void Start()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnAllGeneratorsActivated += HandleAllGeneratorsActivated;
            EventManager_Game.Instance.OnExitComputer += ForceExit;
            EventManager_Game.Instance.OnLocalIsDie += HandlePlayerDeath;
        }
        isAllGeneratorsActivated = false;
        onInteraction = false;
        usingPlayerID = -1;
    }
    public void Interact(int playerId)
    {
        if (isAllGeneratorsActivated == false || onInteraction == true)
        {
            return;
        }

        Vector3 worldPosition = transform.TransformPoint(interactionPoint.localPosition);
        Quaternion worldRotation = interactionPoint.rotation;

        photonView.RPC("SyncStartInteraction", RpcTarget.All, playerId);

        EventManager_Game.Instance.InvokeMoveToComputer(playerId, worldPosition, worldRotation);
        EventManager_Game.Instance.InvokeUseComputer(onInteraction);
    }

    private void ForceExit(int playerID)
    {
        if (usingPlayerID != playerID) return;

        Debug.Log("컴퓨터 강제 종료");
        photonView.RPC("SyncEndInteraction", RpcTarget.All, playerID);

        if (EventManager_Game.Instance != null)
        {
            Debug.Log("이벤트 매니저 호출 성공");
            EventManager_Game.Instance.InvokeUseComputer(onInteraction);
        }
    }
    
    private void HandlePlayerDeath(bool isDead)
    {
        if (isDead && usingPlayerID != -1)
        {
            ForceExit(usingPlayerID);
        }
    }

    [PunRPC]
    private void SyncStartInteraction(int playerID)
    {
        usingPlayerID = playerID;
        onInteraction = true;
    }

    [PunRPC]
    private void SyncEndInteraction(int eventInvokeViewID)
    {
         usingPlayerID = -1;
         onInteraction = false;
    }

    private void HandleAllGeneratorsActivated()
    {
        isAllGeneratorsActivated = true;
        moniterCanvas.gameObject.SetActive(true);
        Debug.Log("컴퓨터 상호작용 활성화.");
    }

    public int GetUsingPlayerID()
    {
        return usingPlayerID;
    }
}
