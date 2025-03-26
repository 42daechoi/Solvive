using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;

public class PlayerVoice : MonoBehaviourPun
{
    Recorder recorder;

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

    void Update()
    {
        if (!photonView.IsMine || recorder == null) return;

        recorder.TransmitEnabled = Input.GetKey(KeyCode.V);
    }
}