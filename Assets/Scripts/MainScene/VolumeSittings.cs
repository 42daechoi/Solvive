using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSittings : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider micVolumeSlider;
    [SerializeField] private AudioMixer voiceMixer;
    private const string MusicVolumeKey = "MusicVolume";
    private const string MicVolumeKey = "MicVolume";

    private void Start()
    {
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
        
        float savedMicVolume = PlayerPrefs.GetFloat(MicVolumeKey, 1f);
        micVolumeSlider.value = savedMicVolume;
        SetMicVolume(savedMicVolume);
        
        MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        micVolumeSlider.onValueChanged.AddListener(SetMicVolume);
    }

    public void SetMusicVolume()
    {
        float volume = MusicSlider.value;

        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            AudioManager.Instance.bgmSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("VolumeSetting: AudioManager 또는 bgmSource가 없습니다.");
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey);
        SetMusicVolume();
    }
    private void SetMicVolume(float value)
    {
        float dB =  Mathf.Clamp(value, -80f, 20f);
        voiceMixer.SetFloat("Master", dB);

        PlayerPrefs.SetFloat(MicVolumeKey, value);
        PlayerPrefs.Save();
    }
}
