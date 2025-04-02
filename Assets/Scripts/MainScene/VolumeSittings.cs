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
        micVolumeSlider.onValueChanged.AddListener(SetMicVolume);
        micVolumeSlider.value = 0f;
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
    private void SetMicVolume(float value)
    {
        voiceMixer.SetFloat("Master", value);
    }
}
