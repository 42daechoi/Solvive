using UnityEngine;

public class SprintState : IState
{
    private float footstepInterval = 0.3f;
    private float footstepTimer = 0f;
    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        playerSound.PlaySprintSound();
        footstepTimer = 0f;
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        if (offset <= 0.21f)
        {
            player.TransitionToState(new CrouchState());
        }
        
        if (offset <= 0.5f)
        {
            player.TransitionToState(new MoveState());
        }
        player.UpdateAnimator();
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            playerSound.PlaySprintSound();
            footstepTimer = 0f;
        }
    }

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
        movement = player.transform.TransformDirection(movement);
        if (!escape)
        {
            movement *= player.SpeedSettings.sprintSpeed;
        }
        else
        {
            movement *= player.localSpeedSettings.sprintSpeed;
        }
        
        
        player.ApplyGravity();
        movement.y = player.VerticalVelocity;
        
        player.Controller.Move(movement * Time.fixedDeltaTime);
        /*Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized * player.SpeedSettings.sprintSpeed;
        movement = player.transform.TransformDirection(movement);
        
        player.Rigidbody.MovePosition(player.Rigidbody.position + movement * Time.fixedDeltaTime);*/
    }

    public void ExitState(PlayerController player)
    {
    }
    
    public bool CanInteraction()
    {
        return true;
    }
}