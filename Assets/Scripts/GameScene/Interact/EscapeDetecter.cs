using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class playerDetecter : MonoBehaviour
{
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
                    Debug.Log("시민");
                    //ui끄고 움직이는 거 끄고 시점만 넘길 수 있게
                    //옵저버 날아다니는거
                }
                else if (pre.role == PlayerRole.Mannequin)
                {
                    Debug.Log("마네킹");
                    //마네킹 => 강화해서 리스폰 수치 딸깍
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
