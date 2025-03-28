using UnityEngine;

public interface IState
{
    void EnterState(PlayerController player, PlayerSound playerSound);
    void UpdateState(PlayerController player, Vector3 inputDirection, float offset, PlayerSound playerSound);
    void FixedUpdateState(PlayerController player, Vector3 inputDirection, float offset, bool escape, PlayerSound playerSound);
    void ExitState(PlayerController player);
    
    
    bool CanInteraction();
}
