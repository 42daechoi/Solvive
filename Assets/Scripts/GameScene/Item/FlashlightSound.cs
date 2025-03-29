using Photon.Pun;
using UnityEngine;

public class FlashlightSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip buttonClickClip;

    private void Start()
    {
        if (TryGetComponent(out AudioSource _audioSource))
        {
            audioSource = _audioSource;
        }
        else
        {
            Debug.LogError("FlashlightSound : AudioSource 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void PlayClickSound()
    {
        photonView.RPC("SyncPlayClickSound", RpcTarget.All);

    }

    [PunRPC]
    private void SyncPlayClickSound()
    {
        audioSource.clip = buttonClickClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
