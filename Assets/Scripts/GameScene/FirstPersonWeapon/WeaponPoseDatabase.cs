using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPoseDatabase", menuName = "ScriptableObjects/WeaponPoseDatabase")]
public class WeaponPoseDatabase : ScriptableObject
{
    public WeaponPoseData[] weaponPoses;
}