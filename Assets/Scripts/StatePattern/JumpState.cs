using UnityEngine;

public class JumpState : IState
{
    private bool hasJumped = false;
    private bool shouldMove = false;
    public void EnterState(PlayerController player)
    {
        Debug.Log("Jump 상태 진입");
        shouldMove = player.GetPreviousState() is MoveState || player.GetPreviousState() is SprintState;
        player.VerticalVelocity = player.SpeedSettings.jumpForce;
        hasJumped = true;
        
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset)
    {
        
    }

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset)
    {
        Vector3 movement = Vector3.zero;
        if (shouldMove)
        {
            movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
            movement = player.transform.TransformDirection(movement);
            float currentSpeed = player.WasInSprintState() ? player.SpeedSettings.sprintSpeed : player.SpeedSettings.walkSpeed;
            movement *= currentSpeed;
        }
        
        if (hasJumped)
        {
            movement.y = player.VerticalVelocity;
            player.Controller.Move(movement * Time.fixedDeltaTime);
            hasJumped = false;
        }
        else
        {
            player.ApplyGravity();
            movement.y = player.VerticalVelocity;
            player.Controller.Move(movement * Time.fixedDeltaTime);
        }

        if (player.IsGrounded() && player.VerticalVelocity <= 0)
        {
            player.TransitionToState(player.WasInSprintState() ? new SprintState() : new IdleState());
        }
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Jump 상태 종료");
    }

    public bool CanInteraction()
    {
        return false;
    }
    
    
}