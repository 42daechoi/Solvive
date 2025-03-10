using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSFlashLight : MonoBehaviour
{
    [SerializeField] private GameObject whiteLight;
    private void OnEnable()
    {
        whiteLight.SetActive(false);
        EventManager_Game.Instance.OnFPSflashlightToggle += HandleFlashlightToggle;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnFPSflashlightToggle -= HandleFlashlightToggle;
    }

    private void HandleFlashlightToggle(bool isOn)
    {
        whiteLight.SetActive(isOn);
    }
}
