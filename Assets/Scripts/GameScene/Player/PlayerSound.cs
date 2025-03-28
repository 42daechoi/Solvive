using UnityEngine;
using Photon.Pun;


public class PlayerSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip sprintClip;
    public AudioClip walkClip;
    public AudioClip jumpClip;
    public AudioClip jumpLandClip;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 10.0f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }


    public void PlayWalkSound()
    {

        photonView.RPC("SyncPlayWalkSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayWalkSound()
    {
        audioSource.maxDistance = 10.0f;
        audioSource.clip = walkClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlaySprintSound()
    {
        photonView.RPC("SyncPlaySprintSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlaySprintSound()
    {
        audioSource.maxDistance = 20.0f;
        audioSource.clip = sprintClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayJumpSound()
    {
        photonView.RPC("SyncPlayJumpSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayJumpSound()
    {
        audioSource.clip = jumpClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayJumpLandSound()
    {
        photonView.RPC("SyncPlayJumpLandSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayJumpLandSound()
    {
        audioSource.clip = jumpLandClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
