using System;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;

public class PlayerVoice : MonoBehaviourPun
{
    Recorder recorder;

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
        if (!photonView.IsMine) return;

        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            Debug.LogError("❌ Recorder 컴포넌트 없음!");
            return;
        }

        string[] micDevices = Microphone.devices;
        if (micDevices.Length == 0)
        {
            Debug.LogError("❌ 마이크 디바이스가 없습니다.");
            return;
        }

        string micName = micDevices[0];
        Debug.Log("🎙️ 마이크 선택됨: " + micName);

        recorder.SourceType = Recorder.InputSourceType.Microphone;
        recorder.MicrophoneType = Recorder.MicType.Unity;

        // ✅ DeviceFeatures 없이 생성
        recorder.MicrophoneDevice = new DeviceInfo(micName, micName);

        recorder.RestartRecording();
        recorder.TransmitEnabled = false;
    }

    void HandleVoice(bool value)
    {
        if (recorder == null)
        {
            Debug.LogWarning("⚠️ Recorder 아직 초기화되지 않음, Transmit 설정 스킵됨");
            return;
        }

        recorder.TransmitEnabled = value;
    }
}