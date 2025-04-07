using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using DG.Tweening;
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
    private bool hasUpdatedAppearance = false;
    
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
            RetryUpdateRole(0.4f, 3);
        }
        
    }

    public void SetRole(PlayerRole newRole)
    {
        photonView.RPC("SetRoleRPC", RpcTarget.All, (int)newRole);
    }
    
    private void RetryUpdateRole(float delay, int Tries)
    {
        if (hasUpdatedAppearance || Tries <= 0)
            return;

        DOVirtual.DelayedCall(delay, () =>
        {
            if (hasUpdatedAppearance)
                return;

            PlayerController[] players = FindObjectsOfType<PlayerController>();
            bool hasOthers = false;
            
            foreach (var p in players)
            {
                if (!p.GetPhotonView().IsMine)
                {
                    hasOthers = true;
                    break;
                }
            }

            if (hasOthers)
            {
                UpdateMannequineColor(players);
                hasUpdatedAppearance = true;
            }
            else
            {
                RetryUpdateRole(delay, Tries - 1);
            }
        }, false);
    }
    
    private void UpdateMannequineColor(PlayerController[] players)
    {
        PlayerRole myRole = this.role;

        foreach (var player in players)
        {
            if (!player.GetPhotonView().IsMine)
            {
                player.MannequineDivide(myRole);
            }
        }
    }
}
