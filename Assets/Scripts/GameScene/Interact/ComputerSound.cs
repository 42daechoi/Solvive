using UnityEngine;
using Photon.Pun;

public class ComputerSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip clickClip;
    public AudioClip errorClip;
    public AudioClip cardSpawnClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("ComputerSound : audioSource를 찾을 수 없습니다.");
        }
    }

    public void PlayClickSound()
    {
        photonView.RPC("SyncPlayClickSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayClickSound()
    {
        audioSource.clip = clickClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayErrorSound()
    {
        photonView.RPC("SyncPlayErrorSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayErrorSound()
    {
        audioSource.clip = errorClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayCardSpawnSound()
    {
        photonView.RPC("SyncPlayCardSpawnSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayCardSpawnSound()
    {
        audioSource.clip = cardSpawnClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
