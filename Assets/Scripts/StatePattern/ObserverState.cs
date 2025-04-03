using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ObserverState : IState
{
    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        CinemachineVirtualCamera fpsCam = player.FPSCam;
        CinemachineVirtualCamera obCam = player.ObserverCam;
        Transform observerTarget = player.ObserverTarget;
        PlayerCamera playerCamera = player.PlayerCamera;
        player.Controller.detectCollisions = false;
        
        fpsCam.gameObject.SetActive(false);
        obCam.gameObject.SetActive(true);
        playerCamera.SetVirtualCamera(obCam);
        playerCamera.SetCamFollowPos(observerTarget);
        
        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("IngameUI");
        foreach (GameObject ui in uiObjects)
        {
            ui.SetActive(false);
        }
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        if (inputDirection.sqrMagnitude < 0.1f)
        {
            return;
        }
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
        movement = player.transform.TransformDirection(movement);
        movement.y = 0;
        movement *= player.SpeedSettings.obSpeed;
        player.Controller.Move(movement * Time.fixedDeltaTime);
        
    }
    

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        
    }

    public void ExitState(PlayerController player)
    {

    }

    public bool CanInteraction()
    {
        return false;
    }
}
