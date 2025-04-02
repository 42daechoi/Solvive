using UnityEngine;

public class IdleState : IState
{
    public void EnterState(PlayerController player, PlayerSound p)
    {
        Debug.Log("Idle자세");
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        player.UpdateAnimator();
        if (offset < 0.21f)
        {
            player.TransitionToState(new CrouchState());
        }
        
        if (inputDirection.sqrMagnitude > 0.1f)
        {
            player.TransitionToState(new MoveState());
        }
    }
    
    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        player.ApplyGravity();
        Vector3 movement = new Vector3(0, player.VerticalVelocity, 0);
        player.Controller.Move(movement * Time.fixedDeltaTime);
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Idle 종료");
    }

    public bool CanInteraction()
    {
        return true;
    }
}