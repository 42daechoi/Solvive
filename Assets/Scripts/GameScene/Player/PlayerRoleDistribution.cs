using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public enum PlayerRole
{
    Citizen,
    Mannequin
}

public class PlayerRoleDistribution : MonoBehaviourPunCallbacks
{
    public PlayerRole role;

    [PunRPC]
    public void SetRoleRPC(int roleInt)
    {
        role = (PlayerRole)roleInt;
        Debug.Log("플레이어의 역할: " + role + photonView.ViewID);
    }

    // 로컬에서 호출할 수 있는 편의 메서드
    public void SetRole(PlayerRole newRole)
    {
        // 모든 클라이언트에 변경 사항을 전파
        photonView.RPC("SetRoleRPC", RpcTarget.All, (int)newRole);
    }
}
