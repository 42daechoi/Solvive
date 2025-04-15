using System;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSittings : MonoBehaviourPun
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider micVolumeSlider;
    [SerializeField] private AudioMixer voiceMixer;
    [SerializeField] private TMP_Dropdown MicModeDropdown;

    private const string MusicVolumeKey = "MusicVolume";
    private const string MicVolumeKey = "MicVolume";
    public int micMode;
    public static VolumeSittings Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (MicModeDropdown != null)
        {
            MicModeDropdown.onValueChanged.AddListener(OnDropdownEvent);
        }
    }

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
        if (PlayerPrefs.HasKey("MicMode"))
        {
            micMode = PlayerPrefs.GetInt("MicMode");
            MicModeDropdown.value = micMode;
        }
        
        float savedMicVolume = PlayerPrefs.GetFloat(MicVolumeKey, 1f);
        micVolumeSlider.value = savedMicVolume;
        SetMicVolume(savedMicVolume);
        
        MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        micVolumeSlider.onValueChanged.AddListener(SetMicVolume);
        micMode = MicModeDropdown.value;
        
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

    private void OnDropdownEvent(int value)
    {
        //if(!photonView.IsMine) return;
        micMode = value;
        PlayerPrefs.SetInt("MicMode", micMode);
        PlayerPrefs.Save();
    }
    
}
