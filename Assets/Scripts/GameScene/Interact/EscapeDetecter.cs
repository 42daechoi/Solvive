using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class playerDetecter : MonoBehaviourPunCallbacks
{
    public SpawnManager spawnManager;
    
    private bool playerInside = false;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("플레이어가 안으로 들어옴");
            PlayerRoleDistribution pre = other.GetComponent<PlayerRoleDistribution>();
            if (pre != null)
            {
                if (pre.role == PlayerRole.Citizen)
                {
                    PhotonView pv = other.GetComponent<PhotonView>();
                    GameManager.Instance.EliminateOrEscapeCitizen(pv.ViewID, "Escape");
                    pre.SetRole(PlayerRole.Observer);
                    /*Debug.Log("시민");
                    // PlayerObserver 컴포넌트를 찾고, 캐릭터랑 UI 제거
                    PlayerObserver observer = other.GetComponent<PlayerObserver>();
                    if (observer != null)
                    {
                        observer.HideCowboy(other.gameObject);
                        pre.SetRole(PlayerRole.Observer);
                    }
                    else
                    {
                        Debug.LogWarning("PlayerObserver 컴포넌트를 찾을 수 없습니다.");
                    }*/
                }
                else if (pre.role == PlayerRole.Mannequin)
                {
                    Debug.Log("마네킹");
                    PlayerController pc = other.GetComponent<PlayerController>(); 
                    if (pc != null)
                    {
                        pc.MannequinEscapeTrigger();
                        spawnManager.RespawnMannequin(other.gameObject);
                    }
                    else
                    {
                        Debug.Log("MovementSettings 컴포넌트를 찾을 수 없음");
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("플레이어 나감");
        }
    }
}
