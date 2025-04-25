using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MICSetting : MonoBehaviour
{
    public TMP_Dropdown micDropdown;
    private string[] micDevices;

    void Start()
    {
        micDevices = Microphone.devices;

        if (micDevices.Length == 0)
        {
            micDropdown.interactable = false;
            return;
        }

        micDropdown.ClearOptions();
        micDropdown.AddOptions(micDevices.ToList());

        micDropdown.onValueChanged.AddListener(OnMicSelected);

        // 기본 마이크 적용
        if (PlayerVoice.Instance != null)
        {
            PlayerVoice.Instance.SetMicrophone(micDevices[0]);
            micDropdown.value = 0;
        }
    }

    void OnMicSelected(int index)
    {
        string selectedMic = micDevices[index];

        if (PlayerVoice.Instance != null)
        {
            PlayerVoice.Instance.SetMicrophone(selectedMic);
        }
        else
        {
            Debug.LogWarning("Mic Setting PlayerVoice.Instance가 null입니다.");
        }
    }
}
