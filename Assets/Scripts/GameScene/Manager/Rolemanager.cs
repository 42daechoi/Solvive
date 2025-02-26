using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Rolemanager : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f); // 플레이어 스폰 대기 시간
        if (PhotonNetwork.IsMasterClient)
        {
            DistributeRoles();
        }
    }

    private void DistributeRoles()
    {
        Debug.Log("역할 분배" );
        // 씬에 있는 모든 플레이어 객체(플레이어 프리팹에 붙은 PlayerRoleDistribution 스크립트)를 찾습니다.
        PlayerRoleDistribution[] players = FindObjectsOfType<PlayerRoleDistribution>();

        if (players.Length == 0)
        {
            Debug.LogWarning("플레이어가 없는데요.");
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
                players[i].SetRole(PlayerRole.Citizen);
            }
        }
    }
}
