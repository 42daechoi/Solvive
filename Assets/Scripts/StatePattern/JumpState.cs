using UnityEngine;

public class JumpState : IState
{
    private bool shouldMove = false;

    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        shouldMove = player.GetPreviousState() is MoveState || player.GetPreviousState() is SprintState;
        player.VerticalVelocity = player.SpeedSettings.jumpForce;
        playerSound.PlayJumpSound();
        Debug.Log("점프상태 들어감");
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        player.UpdateAnimator();
    }

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        Vector3 movement = Vector3.zero;
        
        // 공중에서의 수평 이동 처리
        if (shouldMove)
        {
            movement = new Vector3(inputDirection.x, 0, inputDirection.z).normalized;
            movement = player.transform.TransformDirection(movement);
            float currentSpeed = player.WasInSprintState() ? player.SpeedSettings.sprintSpeed : player.SpeedSettings.walkSpeed;
            movement *= currentSpeed;
        }

        // 수직 이동 처리
        player.ApplyGravity();
        movement.y = player.VerticalVelocity;
        
        // 최종 이동 적용
        player.Controller.Move(movement * Time.fixedDeltaTime);

        // 착지 체크
        if (player.IsGrounded() && player.VerticalVelocity <= 0)
        {
            player.TransitionToState(player.WasInSprintState() ? new SprintState() : new IdleState());
            playerSound.PlayJumpLandSound();
        }
        Debug.Log("점프상태에서의 점프종료");
    }

    public void ExitState(PlayerController player)
    {
        
    }

    public bool CanInteraction()
    {
        return false;
    }
}