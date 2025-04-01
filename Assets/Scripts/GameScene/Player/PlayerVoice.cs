using System;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;

public class PlayerVoice : MonoBehaviourPun
{
    public static PlayerVoice Instance { get; private set; }
    
    Recorder recorder;
    
    private void Awake()
    {
        if (photonView.IsMine && Instance == null)
        {
            Instance = this;
        }
        
        if (!photonView.IsMine) return;

        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            return;
        }

        string[] micDevices = Microphone.devices;
        if (micDevices.Length == 0)
        {
            return;
        }
        
        SetMicrophone(micDevices[0]);

        recorder.TransmitEnabled = false;
    }

    private void OnEnable()
    {
        EventManager_Game.Instance.OnVoice += HandleVoice;
    }

    void OnDisable()
    {
        EventManager_Game.Instance.OnVoice -= HandleVoice;
    }

    void Start()
    {
        
    }
    
    public void SetMicrophone(string micName)
    {
        if (recorder == null)
        {
            Debug.LogWarning("⚠️ Recorder가 아직 초기화되지 않음.");
            return;
        }

        recorder.SourceType = Recorder.InputSourceType.Microphone;
        recorder.MicrophoneType = Recorder.MicType.Unity;
        recorder.MicrophoneDevice = new DeviceInfo(micName, micName);
        recorder.RestartRecording();
    }

    void HandleVoice(bool value)
    {
        if (recorder == null)
        {
            return;
        }

        recorder.TransmitEnabled = value;
    }
}