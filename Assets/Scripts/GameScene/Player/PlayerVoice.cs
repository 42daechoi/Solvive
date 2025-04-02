using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;

public class PlayerVoice : MonoBehaviourPun
{
    public static PlayerVoice Instance { get; private set; }

    private Recorder recorder;
    private Speaker speaker;
    private PlayerRoleDistribution roleDist;

    public PlayerRole Role { get; private set; }

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
        if (recorder == null || roleDist == null) return;

        Role = roleDist.role;

        string[] micDevices = Microphone.devices;
        if (micDevices.Length == 0) return;

        SetMicrophone(micDevices[0]);

        recorder.TransmitEnabled = false;

        roleDist.OnRoleChanged += HandleRoleChanged;
    }

    private void OnEnable()
    {
        EventManager_Game.Instance.OnVoice += HandleVoice;
    }

    private void OnDisable()
    {
        if (roleDist != null)
        {
            roleDist.OnRoleChanged -= HandleRoleChanged;
        }

        EventManager_Game.Instance.OnVoice -= HandleVoice;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(CheckVoice());
        }
    }

    private void HandleRoleChanged(PlayerRole newRole)
    {
        Role = newRole;
    }

    public void SetMicrophone(string micName)
    {
        if (recorder == null)
        {
            Debug.LogWarning("PlayerVoice: ⚠️ Recorder가 아직 초기화되지 않음.");
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

        foreach (var sp in GetComponentsInChildren<Speaker>())
        {
            if (sp.RemoteVoice == null) continue;

            int senderId = sp.RemoteVoice.PlayerId;

            var sender = Array.Find(allPlayers, p => p.photonView.OwnerActorNr == senderId);
            if (sender == null) continue;

            PlayerRole senderRole = sender.Role;
            float distance = Vector3.Distance(transform.position, sender.transform.position);

            bool canHear = false;
            
            if (senderRole == PlayerRole.Observer && this.Role != PlayerRole.Observer)
            {
                canHear = false;
            }
            else if (senderRole == PlayerRole.Observer && this.Role == PlayerRole.Observer)
            {
                canHear = true;
            }
            else
            {
                canHear = distance <= 10f;
            }

            AudioSource audioSource = sp.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = canHear ? 1f : 0f;
            }
        }
    }
}
