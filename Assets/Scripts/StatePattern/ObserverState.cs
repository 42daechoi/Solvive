using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverState : IState
{
    public void EnterState(PlayerController player)
    {
        
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
