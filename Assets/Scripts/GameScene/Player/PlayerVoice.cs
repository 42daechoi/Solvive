using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice;
using UnityEngine.Audio;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerVoice : MonoBehaviourPun
{
    public static PlayerVoice Instance { get; private set; }

    private Recorder recorder;
    private Speaker speaker;
    private PlayerRoleDistribution roleDist;

    [SerializeField] private AudioMixer voiceMixer;
    [SerializeField] private AudioMixerGroup voiceMixerGroup;

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
            HandleRoleChanged(roleDist.role); // 강제 초기화
            SetOutputVolume(+30f);
            StartCoroutine(CheckVoice());
        }
    }

    private void SetOutputVolume(float dB)
    {
        voiceMixer.SetFloat("Volume", dB);
    }

    private void HandleRoleChanged(PlayerRole newRole)
    {
        Role = newRole;
        Debug.Log($"[VOICE] {photonView.Owner.NickName} role changed to {newRole}");

        if (photonView.IsMine)
        {
            Hashtable props = new Hashtable();
            props["Role"] = (int)newRole;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public void SetMicrophone(string micName)
    {
        if (recorder == null)
        {
            Debug.LogWarning("⚠️ Recorder 초기화 안 됨");
            return;
        }

        recorder.SourceType = Recorder.InputSourceType.Microphone;
        recorder.MicrophoneType = Recorder.MicType.Unity;
        recorder.MicrophoneDevice = new DeviceInfo(micName, micName);
        recorder.RestartRecording();
    }

    void HandleVoice(bool value)
    {
        if (recorder == null) return;

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
        var allPlayers = FindObjectsOfType<PlayerVoice>();
        var allSpeakers = FindObjectsOfType<Speaker>();

        foreach (var sp in allSpeakers)
        {
            if (sp.RemoteVoice == null) continue;

            int senderId = sp.RemoteVoice.PlayerId;
            var sender = allPlayers.FirstOrDefault(p => p.photonView.OwnerActorNr == senderId);
            if (sender == null) continue;

            float distance = Vector3.Distance(transform.position, sender.transform.position);

            // ✅ sender.Role 대신 CustomProperties에서 역할 가져오기
            PlayerRole senderRole = GetRoleOfPlayer(sender.photonView.Owner);
            bool canHear = ShouldHearThisPlayer(senderRole, this.Role, distance);

            // 디버그 출력
            Debug.Log($"[VOICE] I({photonView.Owner.NickName}, {Role}) {(canHear ? "CAN" : "CANNOT")} hear {sender.photonView.Owner.NickName}, {senderRole}, Dist={distance:F2}");

            AudioSource audioSource = sp.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.outputAudioMixerGroup = voiceMixerGroup;
                audioSource.volume = canHear ? 1f : 0f;
                audioSource.mute = !canHear;
            }

            sp.enabled = canHear;

            // 💥 연결 강제 해제
            if (!canHear)
            {
                var unlinkMethod = typeof(Speaker).GetMethod("Unlink", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                unlinkMethod?.Invoke(sp, null);
            }
        }
    }

    private PlayerRole GetRoleOfPlayer(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.TryGetValue("Role", out object roleValue))
        {
            return (PlayerRole)(int)roleValue;
        }

        return PlayerRole.Citizen; // 기본값
    }

    private bool ShouldHearThisPlayer(PlayerRole senderRole, PlayerRole listenerRole, float distance)
    {
        if (senderRole == PlayerRole.Observer && listenerRole != PlayerRole.Observer)
            return false;

        if (senderRole == PlayerRole.Observer && listenerRole == PlayerRole.Observer)
            return true;

        return distance <= 10f;
    }
}
