using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSittings : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider MusicSlider;

    private void Start()
    {
        // audioSource가 할당되지 않았다면, 같은 게임 오브젝트에서 AudioSource 컴포넌트를 가져옵니다.
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (PlayerPrefs.HasKey("MainBGM"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MainBGM", volume);
    }

    private void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MainBGM");
        SetMusicVolume();
    }
}
