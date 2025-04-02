using UnityEngine;

public class CrouchState : IState
{
    private float footstepInterval = 0.7f;
    private float footstepTimer = 0f;
    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        Debug.Log("앉기 test");
    }
    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        player.UpdateAnimator();
        
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
            playerSound.PlayWalkSound();
            footstepTimer = 0f;
        }
    }
    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        
    }
    public void ExitState(PlayerController player)
    {
        Debug.Log("앉기 끝 test");
    }


    public bool CanInteraction()
    {
        return true;
    }
}
