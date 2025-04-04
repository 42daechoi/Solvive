using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;
using Photon.Voice.PUN;

public class PlayerVoice : MonoBehaviourPun
{
    Recorder recorder;
    PunVoiceClient punVoiceClient;
    bool groupChanged = false;

    private void OnEnable()
    {
        EventManager_Game.Instance.OnVoice += HandleVoice;
        EventManager_Game.Instance.OnEliminateOrEscape += HandleVoiceGroup;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnVoice -= HandleVoice;
        EventManager_Game.Instance.OnEliminateOrEscape -= HandleVoiceGroup;

        if (punVoiceClient != null)
        {
            punVoiceClient.Client.StateChanged -= OnVoiceStateChanged;
        }
    }

    void Start()
    {
        if (!photonView.IsMine) return;

        punVoiceClient = GameObject.Find("VoiceManager")?.GetComponent<PunVoiceClient>();
        Debug.Log($"PlayerVoice: PunvoiceClient - {punVoiceClient}");

        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            Debug.LogError("PlayerVoice : Recorder 컴포넌트 없음!");
            return;
        }

        recorder.InterestGroup = 1;
        recorder.TransmitEnabled = false;

        string[] micDevices = Microphone.devices;
        if (micDevices.Length == 0)
        {
            Debug.LogError("PlayerVoice : 마이크 디바이스가 없습니다.");
            return;
        }

        string micName = micDevices[0];
        Debug.Log("PlayerVoice :  마이크 선택됨: " + micName);

        recorder.SourceType = Recorder.InputSourceType.Microphone;
        recorder.MicrophoneType = Recorder.MicType.Unity;
        recorder.MicrophoneDevice = new DeviceInfo(micName, micName);
        recorder.RestartRecording();

        // 🔥 상태 변화 감지 시작
        punVoiceClient.Client.StateChanged += OnVoiceStateChanged;
    }

    private void OnVoiceStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        if (toState == Photon.Realtime.ClientState.Joined && !groupChanged)
        {
            byte[] receiveGroups = new byte[] { 1 };
            punVoiceClient.Client.OpChangeGroups(null, receiveGroups);
            groupChanged = true;

            Debug.Log("PlayerVoice: 그룹 변경 완료 (Group 1 수신)");
        }
    }

    void HandleVoice(bool value)
    {
        if (!photonView.IsMine) return;
        if (recorder == null)
        {
            Debug.LogWarning("PlayerVoice : Recorder 아직 초기화되지 않음, Transmit 설정 스킵됨");
            return;
        }

        recorder.TransmitEnabled = value;
    }

    private void HandleVoiceGroup(string flag)
    {
        if (!photonView.IsMine) return;
        recorder.InterestGroup = 2;

        byte[] receiveGroups = new byte[] { 1, 2 };
        punVoiceClient.Client.OpChangeGroups(null, receiveGroups);

        Debug.Log("PlayerVoice: Group 2로 송신, Group 1,2 수신 설정됨");
    }
}
