using UnityEngine;

public class MoveState : IState
{
    private float footstepInterval = 0.5f;
    private float footstepTimer = 0f;

    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        Debug.Log("Move행동 진입");
        playerSound.PlayWalkSound();
        footstepTimer = 0f;
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        player.UpdateAnimator();
        
        if (offset <= 0.21f)
        {
            player.TransitionToState(new CrouchState());
        }
        
        if (offset > 0.5f)
        {
            player.TransitionToState(new SprintState());
        }

        if (inputDirection.sqrMagnitude < 0.1f)
        {
            player.TransitionToState(new IdleState());
        }
        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            playerSound.PlayWalkSound();
            footstepTimer = 0f;
        }
    }

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        player.ApplyGravity();
        Vector3 movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
        movement = player.transform.TransformDirection(movement);
        movement.y = player.VerticalVelocity;
        if (!escape)
        {
            movement *= player.SpeedSettings.walkSpeed;
        }
        else
        {
            movement *= player.localSpeedSettings.walkSpeed;
        }
        player.Controller.Move(movement * Time.fixedDeltaTime);
        
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Move행동 벗어남");
    }
    
    public bool CanInteraction()
    {
        return true;
    }
}