using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunBullet : MonoBehaviourPun
{
    private int bulletCount;

    private void Start()
    {
        bulletCount = 2;
    }

    public bool TryGunShoot()
    {
        if (bulletCount <= 0) return false;
        else
        {
            photonView.RPC("SyncBulletCount", RpcTarget.All);
            return true;
        }
    }

    [PunRPC]
    private void SyncBulletCount()
    {
        bulletCount--;
    }

}
