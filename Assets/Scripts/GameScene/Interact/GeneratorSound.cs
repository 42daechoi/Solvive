using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip startClip;
    public AudioClip loopClip;
    public AudioClip endClip;

    private void Start()
    {
        if (TryGetComponent(out AudioSource _audioSource))
        {
            audioSource = _audioSource;
        }
        else
        {
            Debug.LogError("GeneratorSound : AudioSource 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void PlayStartSound()
    {
        audioSource.clip = startClip;
        audioSource.loop = false;
        audioSource.Play();

        Invoke(nameof(PlayLoopSound), startClip.length);
    }

    private void PlayLoopSound()
    {
        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayEndSound()
    {
        audioSource.Stop();
        audioSource.clip = endClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
