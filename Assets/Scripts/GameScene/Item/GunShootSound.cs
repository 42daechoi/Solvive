using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunShootSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip missShootClip;

    private void Start()
    {
        if (TryGetComponent(out AudioSource _audioSource))
        {
            audioSource = _audioSource;
        }
        else
        {
            Debug.LogError("Gun : AudioSource 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void PlayShootSound()
    {
        photonView.RPC("SyncPlayShootSound", RpcTarget.All);

    }

    [PunRPC]
    private void SyncPlayShootSound()
    {
        audioSource.clip = shootClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayMissShootSound()
    {
        photonView.RPC("SyncPlayMissShootSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayMissShootSound()
    {
        audioSource.clip = missShootClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
