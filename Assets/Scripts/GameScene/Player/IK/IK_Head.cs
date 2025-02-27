using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Head : MonoBehaviour
{
    public Transform _target;
    private Quaternion _initialRotation;
    
    void Start()
    {
        _initialRotation = transform.localRotation;
    }
    // Start is called before the first frame update
    void LateUpdate()
    {
        if (_target == null) return;
        
        Vector3 localTargetPos = transform.parent.InverseTransformPoint(_target.position);
        Vector3 localHeadPos = transform.parent.InverseTransformPoint(transform.position);
        
        
        Vector3 direction = localTargetPos - localHeadPos;
        
        
        float xAngle = -1 * Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
        
        
        xAngle = Mathf.Clamp(xAngle, -45f, 45f);
        
        
        transform.localRotation = _initialRotation * Quaternion.Euler(xAngle, 0, 0);
    }
}
