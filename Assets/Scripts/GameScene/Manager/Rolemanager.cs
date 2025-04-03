using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoleManager : MonoBehaviour
{
    private int citizenCount = 0;
    [SerializeField] private int mannequinCount = 1;
    private HashSet<int> mannequinIndexSet = new HashSet<int>();

    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnAllPlayerSpawned += DistributeRoles;
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MannequinCount"))
        {
            mannequinCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["MannequinCount"];
        }
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

        PlayerRoleDistribution[] players = FindObjectsOfType<PlayerRoleDistribution>();
        if (players.Length == 0) return;

        while (mannequinIndexSet.Count < mannequinCount)
        {
            int randomIndex = Random.Range(0, players.Length);
            mannequinIndexSet.Add(randomIndex);
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (mannequinIndexSet.Contains(i))
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
