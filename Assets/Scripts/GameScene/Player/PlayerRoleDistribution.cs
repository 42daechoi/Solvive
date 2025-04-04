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
    Mannequin,
    Observer
}

public class PlayerRoleDistribution : MonoBehaviourPunCallbacks, IPunObservable
{
    public PlayerRole role;
    public event Action<PlayerRole> OnRoleChanged;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)role);
        }
        else
        {
            role = (PlayerRole)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SetRoleRPC(int roleInt)
    {
        role = (PlayerRole)roleInt;
        OnRoleChanged?.Invoke(role);
        Debug.Log("플레이어의 역할: " + role + photonView.ViewID);
        if (photonView.IsMine) 
        {
            RoleUI.Instance.UpdateRoleUI(role);
            EventManager_Game.Instance.InvokeSetRoleComplete(role);
        }
    }

    public void SetRole(PlayerRole newRole)
    {
        photonView.RPC("SetRoleRPC", RpcTarget.All, (int)newRole);
    }
}
