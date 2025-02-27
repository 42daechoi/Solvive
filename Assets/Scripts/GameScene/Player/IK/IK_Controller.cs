using UnityEngine;
using DitzelGames.FastIK;
public class IK_Controller : MonoBehaviour
{
    private FastIKFabric[] _ikComponents;
    
    [SerializeField] private Transform _rightHandTarget;
    [SerializeField] private Transform _leftHandTarget;
    private void Awake()
    {
        _ikComponents = GetComponentsInChildren<FastIKFabric>();
        
        EnableIK(false);
    }
    
    public void EnableIK(bool enable)
    {
        foreach (var ik in _ikComponents)
        {
            ik.enabled = enable;
        }
    }

    public void SetIKTarget(Vector3 rightHandPos, Vector3 rightHandRot, Vector3 leftHandPos, Vector3 leftHandRot)
    {
        _rightHandTarget.localPosition = rightHandPos;
        _rightHandTarget.localRotation = Quaternion.Euler(rightHandRot);
        
        _leftHandTarget.localPosition = leftHandPos;
        _leftHandTarget.localRotation = Quaternion.Euler(leftHandRot);
    }

    public void SetEnableIK(Vector3 rightHandPos, Vector3 rightHandRot, Vector3 leftHandPos, Vector3 leftHandRot)
    {
        SetIKTarget(rightHandPos, rightHandRot, leftHandPos, leftHandRot);
        EnableIK(true);
    }

    public void DisableIK()
    {
        EnableIK(false);
    }
}
