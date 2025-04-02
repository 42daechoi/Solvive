using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;

public class PlayerVoice : MonoBehaviourPun
{
    public static PlayerVoice Instance { get; private set; }
    private Speaker speaker;
    private PlayerRoleDistribution roleDist;
    public PlayerRole Role { get; private set; }
    
    Recorder recorder;
    
    private void Awake()
    {
        if (photonView.IsMine && Instance == null)
        {
            Instance = this;
        }
        recorder = GetComponent<Recorder>();
        speaker = GetComponent<Speaker>();
        roleDist = GetComponent<PlayerRoleDistribution>();
        
        if (!photonView.IsMine) return;
        
        if (recorder == null || roleDist == null)
        {
            return;
        }
        Role = roleDist.role;

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
        if (photonView.IsMine)
        {
            StartCoroutine(CheckVoice());
        }
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
    
    private IEnumerator CheckVoice()
    {
        while (true)
        {
            UpdateCanHear();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void UpdateCanHear()
    {
        PlayerVoice[] allPlayers = FindObjectsOfType<PlayerVoice>();

        foreach (var other in allPlayers)
        {
            if (other == this || other.speaker == null) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            bool canHear = false;

            if (this.Role == PlayerRole.Observer && other.Role == PlayerRole.Observer)
            {
                canHear = true;
            }
            else if (this.Role == PlayerRole.Observer || other.Role == PlayerRole.Observer)
            {
                canHear = false;
            }
            else
            {
                canHear = distance <= 10f;
            }

            if (speaker != null && speaker.RemoteVoice != null &&
                speaker.RemoteVoice.PlayerId == other.photonView.OwnerActorNr)
            {
                speaker.enabled = canHear;
            }
        }
    }
}