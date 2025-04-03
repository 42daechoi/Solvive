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
        
        int observerLayer = LayerMask.NameToLayer("Observer");
        player.gameObject.layer = observerLayer;
        
        fpsCam.gameObject.SetActive(false);
        obCam.gameObject.SetActive(true);
        playerCamera.SetVirtualCamera(obCam);
        playerCamera.SetCamFollowPos(observerTarget);
        
        if (player.GetPhotonView().IsMine)
        {
            GameObject[] uiObjects = player.IngameUIObjects;
            foreach (GameObject ui in uiObjects)
            {
                ui.SetActive(false);
            }
        }
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        if (inputDirection.sqrMagnitude < 0.1f)
        {
            return;
        }
    }
    

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        player.ApplyGravity();
        if (player.IsGrounded() && player.IsJump)
        {
            player.VerticalVelocity = player.SpeedSettings.jumpForce;
        }

        Vector3 movement = Vector3.zero;

        if (inputDirection.sqrMagnitude >= 0.1f)
        {
            movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
            movement = player.transform.TransformDirection(movement);
            movement *= player.SpeedSettings.obSpeed;
        }

        movement.y = player.VerticalVelocity;

        player.Controller.Move(movement * Time.fixedDeltaTime);
        player.IsJump = false;
    }

    public void ExitState(PlayerController player)
    {

    }

    public bool CanInteraction()
    {
        return false;
    }
}
