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
    public AudioClip hitClip;
    private bool isMouthPlaying = false;


    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.minDistance = 1.0f;
            audioSource.maxDistance = 10.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    private void Start()
    {
        EventManager_Game.Instance.OnTakeDamage += PlayHitSound;
    }

    private void Disable()
    {
        EventManager_Game.Instance.OnTakeDamage -= PlayHitSound;
    }


    public void PlayWalkSound()
    {
        photonView.RPC("SyncPlayWalkSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayWalkSound()
    {
        audioSource.maxDistance = 10.0f;
        audioSource.volume = 1f;
        audioSource.PlayOneShot(walkClip);
    }

    public void PlayCrouchSound()
    {
        photonView.RPC("SyncPlayCrouchSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlayCrouchSound()
    {
        audioSource.maxDistance = 5.0f;
        audioSource.volume = 0.6f;
        audioSource.PlayOneShot(walkClip);
    }


    public void PlaySprintSound()
    {
        photonView.RPC("SyncPlaySprintSound", RpcTarget.All);
    }

    [PunRPC]
    private void SyncPlaySprintSound()
    {
        audioSource.volume = 1f;
        audioSource.maxDistance = 20.0f;
        audioSource.PlayOneShot(sprintClip);
    }



    public void PlayPantingSound()
    {
        if (!isMouthPlaying)
        {
            photonView.RPC("SyncPlayPantingSound", RpcTarget.All);
            isMouthPlaying = true;
            Invoke(nameof(ResetMouthFlag), pantingClip.length);
        }
    }

    [PunRPC]
    private void SyncPlayPantingSound()
    {
        audioSource.maxDistance = 20.0f;
        audioSource.PlayOneShot(pantingClip);
    }

    private void ResetMouthFlag()
    {
        isMouthPlaying = false;
    }

    public void PlayHitSound()
    {
        if (!isMouthPlaying)
        {
            photonView.RPC("SyncPlayHitSound", RpcTarget.All);
            isMouthPlaying = true;
            Invoke(nameof(ResetMouthFlag), hitClip.length);
        }
    }

    [PunRPC]
    private void SyncPlayHitSound()
    {
        audioSource.maxDistance = 10.0f;
        audioSource.PlayOneShot(hitClip);
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
