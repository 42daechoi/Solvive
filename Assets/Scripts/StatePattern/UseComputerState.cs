using UnityEngine;

public class UseComputerState : IState
{
    public void EnterState(PlayerController player)
    {
        Debug.Log("UseComputerState에 진입했습니다.");
        player.StartMoveToComputer();
    }

    public void UpdateState(PlayerController player, Vector3 inputDirection, float offset)
    {
        
    }
    

    public void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape)
    {
        
    }

    public void ExitState(PlayerController player)
    {

    }

    public bool CanInteraction()
    {
        return false;
    }
}
