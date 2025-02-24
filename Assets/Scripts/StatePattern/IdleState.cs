using UnityEngine;

public class IdleState : IState
{
    public void EnterState(PlayerController player)
    {
        Debug.Log("Idle자세");
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset)
    {
        player.UpdateAnimator();
        if (inputDirection.sqrMagnitude > 0.1f)
        {
            player.TransitionToState(new MoveState());
        }
    }
    
    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset)
    {
        player.UpdateAnimator();
        player.ApplyGravity();
        Vector3 movement = new Vector3(0, player.VerticalVelocity, 0);
        player.Controller.Move(movement * Time.fixedDeltaTime);
    }

    public void ExitState(PlayerController player)
    {
        
    }

    public bool CanInteraction()
    {
        return true;
    }
}