using Photon.Pun;
using UnityEngine;

public class ObserverRender : MonoBehaviourPun
{
    public GameObject observerObject;
    private PlayerController[] playerControllers;

    private void Start()
    {
        EventManager_Game.Instance.OnAllPlayerSpawned += FindPlayerControllers;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnAllPlayerSpawned -= FindPlayerControllers;
    }

    private void FindPlayerControllers()
    {
        playerControllers = FindObjectsOfType<PlayerController>();
    }

    public void SetActiveRender()
    {
        // 이미 RPC 내부임, 즉 각 로컬의 옵저버가 될 플레이어의 렌더

        if (photonView.IsMine)
        {
            ActiveOthersWhenLocalChangeToObserver();
        }
        else
        {
            ActiveThisWhenLocalIsObserver();
        }

    }

    private void ActiveThisWhenLocalIsObserver()
    {
        // 옵저버가 될 사람의 렌더를 옵저버인 다른 플레이어들 로컬에서 키는 역할
        foreach (var pc in playerControllers)
        {
            if (pc == null) continue;
            if (pc.GetPhotonView().IsMine && pc.GetCurrentState() is ObserverState)
            {
                observerObject.SetActive(true);
            }
        }
    }


    private void ActiveOthersWhenLocalChangeToObserver()
    {
        // 옵저버가 될 사람이 나일 때 이미 옵저버가 되어있던 사람들의 렌더를 내 로컬에서 키는 역할

        foreach (var pc in playerControllers)
        {
            if (pc == null) continue;
            if (pc.GetCurrentState() is ObserverState)
            {
                ObserverRender or = pc.GetComponent<ObserverRender>();
                if (or != null)
                {
                    or.observerObject.SetActive(true);
                }
            }
        }
    }
}
