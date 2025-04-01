using UnityEngine;

public class UseComputerState : IState
{
    public void EnterState(PlayerController player, PlayerSound playerSound)
    {
        Debug.Log("UseComputerState에 진입했습니다.");
        player.StartMoveToComputer();
        player.PlayerAnimator.EnterComputerAnim();
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound)
    {
        
    }
    

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound)
    {
        
    }

    public void ExitState(PlayerController player)
    {
        player.PlayerAnimator.ExitComputerAnim();
    }

    public bool CanInteraction()
    {
        return false;
    }
}
