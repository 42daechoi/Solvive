using UnityEngine;

public class CrouchState : IState
{
    private float footstepInterval = 0.7f;
    private float footstepTimer = 0f;
    private Vector3 lastInputDirection = Vector3.zero;
    
    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        player.PlayerAnimator.SetCrouchLayerActive(true);
        playerSound.PlayCrouchSound();
        footstepTimer = 0f;
    }
    
    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        if (inputDirection.sqrMagnitude < 0.05f)
        {
            inputDirection = Vector3.zero;
        }
        
        if (inputDirection != lastInputDirection)
        {
            lastInputDirection = inputDirection;
            player.UpdateAnimator();
        }
        
        if (offset > 0.21f)
        {
            if (inputDirection.sqrMagnitude > 0.1f)
                player.TransitionToState(new MoveState());
            else
                player.TransitionToState(new IdleState());

            return;
        }
        
        footstepTimer += Time.deltaTime;
        if (inputDirection.sqrMagnitude > 0.1f && footstepTimer >= footstepInterval)
        {
            playerSound.PlayCrouchSound();
            footstepTimer = 0f;
        }
    }
    
    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        player.ApplyGravity();

        if (inputDirection.sqrMagnitude < 0.05f)
        {
            inputDirection = Vector3.zero;
        }

        Vector3 movement = inputDirection.normalized;
        movement = player.transform.TransformDirection(movement);
        movement.y = player.VerticalVelocity;

        float crouchSpeed = player.SpeedSettings.walkSpeed * 0.4f;
        movement *= crouchSpeed;

        player.Controller.Move(movement * Time.fixedDeltaTime);
    }
    
    public void ExitState(PlayerController player)
    {
        player.PlayerAnimator.SetCrouchLayerActive(false);
    }

    public bool CanInteraction()
    {
        return true;
    }
}