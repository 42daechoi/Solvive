using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KnifeStabSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip stabClip;

    private void Start()
    {
        if (TryGetComponent(out AudioSource _audioSource))
        {
            audioSource = _audioSource;
            audioSource.clip = stabClip;
            audioSource.loop = false;
        }
        else
        {
            Debug.LogError("Knife : AudioSource 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void PlayStabSound()
    {
        photonView.RPC("SyncPlayStabSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayStabSound()
    {
        audioSource.Play();
    }
}
