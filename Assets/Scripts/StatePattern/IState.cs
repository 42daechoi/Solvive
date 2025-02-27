using UnityEngine;

public interface IState
{
    void EnterState(PlayerController player);
    void UpdateState(PlayerController player, Vector3 inputDirection, float offset);
    void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape);
    void ExitState(PlayerController player);
    
    
    bool CanInteraction();
}
