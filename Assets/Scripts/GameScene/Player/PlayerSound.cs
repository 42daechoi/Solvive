using UnityEngine;
using Photon.Pun;

public class PlayerSound : MonoBehaviourPun
{
    public AudioSource audioSource;
    public AudioClip sprintClip;
    public AudioClip walkClip;
    public AudioClip jumpClip;
    public AudioClip jumpLandClip;
    public AudioClip pantingClip;
    private bool isPantingPlaying = false;

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
        audioSource.PlayOneShot(walkClip);
    }


    public void PlaySprintSound()
    {
        photonView.RPC("SyncPlaySprintSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlaySprintSound()
    {
        audioSource.maxDistance = 20.0f;
        audioSource.PlayOneShot(sprintClip);
    }



    public void PlayPantingSound()
    {
        if (!isPantingPlaying)
        {
            photonView.RPC("SyncPlayPantingSound", RpcTarget.All);
            isPantingPlaying = true;
            Invoke(nameof(ResetPantingFlag), pantingClip.length);
        }
    }

    [PunRPC]
    private void SyncPlayPantingSound()
    {
        audioSource.maxDistance = 20.0f;
        audioSource.PlayOneShot(pantingClip);
    }

    private void ResetPantingFlag()
    {
        isPantingPlaying = false;
    }


    public void PlayJumpSound()
    {
        photonView.RPC("SyncPlayJumpSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayJumpSound()
    {
        audioSource.PlayOneShot(jumpClip);
    }


    public void PlayJumpLandSound()
    {
        photonView.RPC("SyncPlayJumpLandSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayJumpLandSound()
    {
        audioSource.PlayOneShot(jumpLandClip);
    }
}
