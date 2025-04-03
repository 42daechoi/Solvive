using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Game : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip oneCitizenAloneClip;
    public AudioClip computerPhaseClip;

    private void Start()
    {
        TryGetComponent(out audioSource);
        if (audioSource == null)
        {
            Debug.LogError("AudioManager_Game : AudioSource를 찾을 수 없습니다.");
            return;
        }

        audioSource.spatialBlend = 0f;
        audioSource.loop = true;

        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOneCitizenAlive += PlayOneCitizenAloneClip;
            EventManager_Game.Instance.OnAllGeneratorsActivated += PlayComputerPhaseClip;
        }
    }

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOneCitizenAlive -= PlayOneCitizenAloneClip;
            EventManager_Game.Instance.OnAllGeneratorsActivated -= PlayComputerPhaseClip;
        }
    }

    private void PlayOneCitizenAloneClip()
    {
        if (audioSource.clip == oneCitizenAloneClip) return;

        audioSource.clip = oneCitizenAloneClip;
        audioSource.Play();
    }

    private void PlayComputerPhaseClip()
    {
        audioSource.PlayOneShot(computerPhaseClip);
    }
}
