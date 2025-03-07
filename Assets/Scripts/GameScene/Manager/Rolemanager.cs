using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class RoleManager : MonoBehaviour
{
    private int citizenCount = 0;

    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnAllPlayerSpawned += DistributeRoles;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForEventManager());
    }

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnAllPlayerSpawned -= DistributeRoles;
        }
    }

    private void DistributeRoles()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // 씬에 있는 모든 플레이어 객체(플레이어 프리팹에 붙은 PlayerRoleDistribution 스크립트)를 찾습니다.
        PlayerRoleDistribution[] players = FindObjectsOfType<PlayerRoleDistribution>();

        if (players.Length == 0)
        {
            return;
        }
        // 랜덤으로 한 명을 마네킹으로 선택합니다.
        int mannequinIndex = Random.Range(0, players.Length);


        for (int i = 0; i < players.Length; i++)
        {
            if (i == mannequinIndex)
            {
                players[i].SetRole(PlayerRole.Mannequin);
            }
            else
            {
                citizenCount++;
                players[i].SetRole(PlayerRole.Citizen);
            }
        }
        GameManager.Instance.InitCitizenCount(citizenCount);
    }
}
