using UnityEngine;

[CreateAssetMenu(fileName = "MovementSettings", menuName = "Settings/MovementSettings", order = 1)]
public class MovementSettings : ScriptableObject
{
    [Header("Basic Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    
    [Header("Jump Settings")]
    public float jumpForce = 15f;
    
    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float groundedGravity = -1f;
}
