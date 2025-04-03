using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EscapeDetecter : MonoBehaviourPunCallbacks
{
    public SpawnManager spawnManager;

    public void OnPlayerEnteredDetector(Collider other, int detectorIndex)
    {
        if (!other.CompareTag("Player")) return;

        PlayerRoleDistribution pre = other.GetComponent<PlayerRoleDistribution>();
        if (pre == null) return;

        if (pre.role == PlayerRole.Citizen)
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            GameManager.Instance.EliminateOrEscapeCitizen(pv.ViewID, "Escape");
            pre.SetRole(PlayerRole.Observer);
            spawnManager.RespawnObserver(other.gameObject, detectorIndex);
        }
        else if (pre.role == PlayerRole.Mannequin)
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.MannequinEscapeTrigger();
                spawnManager.RespawnMannequin(other.gameObject);
            }
        }
    }
}
