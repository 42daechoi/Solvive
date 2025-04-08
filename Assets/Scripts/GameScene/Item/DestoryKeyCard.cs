using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DestoryKeyCard : MonoBehaviourPunCallbacks
{
    [PunRPC]
    public void RPC_Destroy()
    {
        if (photonView.IsMine || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void RequestDestroy()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_Destroy", RpcTarget.AllBuffered);
        }
    }
}