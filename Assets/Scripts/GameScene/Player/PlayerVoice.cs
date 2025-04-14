using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;
using Photon.Voice.PUN;
using UnityEngine.Audio;
using System.Collections;

public class PlayerVoice : MonoBehaviourPun
{
    public Recorder recorder;
    public AudioSource audioSource;
    public PunVoiceClient punVoiceClient;
    [SerializeField] private AudioMixer voiceMixer;
    [SerializeField] private AudioMixerGroup voiceMixerGroup;
    public int micMode;

    public static PlayerVoice Instance { get; private set; }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        EventManager_Game.Instance.OnVoice += HandleVoice;
        EventManager_Game.Instance.OnEliminateOrEscape += HandleVoiceGroup;
        EventManager_Game.Instance.OnAllPlayerSpawned += Init;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnVoice -= HandleVoice;
        EventManager_Game.Instance.OnEliminateOrEscape -= HandleVoiceGroup;
        EventManager_Game.Instance.OnAllPlayerSpawned -= Init;
    }

    void Init()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        FindComponents();
        micMode = VolumeSittings.Instance.micMode;
        SetOutputVolume(+20f);

        recorder.InterestGroup = 1;
        recorder.TransmitEnabled = false;

        // 그룹 변경 시도 코루틴 시작
        StartCoroutine(WaitForVoiceReadyAndJoinGroup(new byte[] { 1 }));

        // 마이크 설정
        string[] micDevices = Microphone.devices;
        if (micDevices.Length == 0)
        {
            Debug.LogError("PlayerVoice : 마이크 디바이스가 없습니다.");
        }
        else
        {
            string micName = micDevices[0];
            Debug.Log("PlayerVoice : 마이크 선택됨: " + micName);

            recorder.SourceType = Recorder.InputSourceType.Microphone;
            recorder.MicrophoneType = Recorder.MicType.Unity;
            recorder.MicrophoneDevice = new DeviceInfo(micName, micName);
            recorder.RestartRecording();
        }
    }

    IEnumerator WaitForVoiceReadyAndJoinGroup(byte[] receiveGroups)
    {
        // GameServer (룸 입장 완료) 상태까지 대기
        while (!punVoiceClient.Client.InRoom)
        {
            yield return null;
        }

        punVoiceClient.Client.OpChangeGroups(null, receiveGroups);
        Debug.Log("PlayerVoice : 그룹 변경 완료.");
    }

    void HandleVoice(bool value)
    {
        if (!photonView.IsMine) return;

        if (recorder == null)
        {
            Debug.LogWarning("PlayerVoice : Recorder 아직 초기화되지 않음, Transmit 설정 스킵됨");
            return;
        }

        micMode = VolumeSittings.Instance.micMode;

        if (micMode == 0)
        {
            recorder.TransmitEnabled = value;
        }
        else if (micMode == 1)
        {
            recorder.TransmitEnabled = true;
        }
    }

    private void HandleVoiceGroup(string flag)
    {
        if (!photonView.IsMine) return;

        bool isObserver = flag == "Eliminate" || flag == "Escape";

        recorder.InterestGroup = isObserver ? (byte)2 : (byte)1;
        byte[] receiveGroups = isObserver ? new byte[] { 1, 2 } : new byte[] { 1 };

        // InRoom 상태에서만 변경
        if (punVoiceClient.Client.InRoom)
        {
            punVoiceClient.Client.OpChangeGroups(null, receiveGroups);
            Debug.Log("PlayerVoice : 관전자 그룹 변경 완료.");
        }
        else
        {
            Debug.LogWarning("PlayerVoice : 아직 룸에 입장하지 않아 그룹 변경이 불가능합니다.");
        }

        if (isObserver)
        {
            audioSource.spatialBlend = 0f;
        }
    }

    private void SetOutputVolume(float dB)
    {
        voiceMixer.SetFloat("Volume", dB);
    }

    public void SetMicrophone(string micName)
    {
        if (!photonView.IsMine) return;

        if (recorder == null)
        {
            Debug.LogWarning("PlayerVoice : Recorder가 없습니다.");
            return;
        }

        recorder.MicrophoneDevice = new DeviceInfo(micName, micName);
        recorder.RestartRecording();

        Debug.Log($"PlayerVoice : 마이크 변경됨 => {micName}");
    }

    private void FindComponents()
    {
        if (!photonView.IsMine) return;

        if (recorder == null)
        {
            recorder = GetComponent<Recorder>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (punVoiceClient == null)
        {
            punVoiceClient = GameObject.Find("VoiceManager")?.GetComponent<PunVoiceClient>();
        }
    }
}
