using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Head : MonoBehaviour
{
    public Transform Target;
    protected Vector3 StartDirection;
    protected Quaternion StartRotation;
    
    void Awake()
    {
        if (Target == null)
            return;

        StartDirection = Target.position - transform.position;
        StartRotation = transform.rotation;
    }
    // Start is called before the first frame update
    void LateUpdate()
    {
        if (Target == null)
            return;

        transform.rotation = Quaternion.FromToRotation(StartDirection, Target.position - transform.position) * StartRotation;
    }
}
