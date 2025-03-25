using UnityEngine;

[CreateAssetMenu(fileName = "WeaponPoseDatabase", menuName = "ScriptableObjects/WeaponPoseDatabase")]
public class WeaponPoseDatabase : ScriptableObject
{
    public WeaponPoseData[] weaponPoses;
    
    public WeaponPoseData GetPose(string weaponName)
    {
        foreach (var pose in weaponPoses)
        {
            if (pose.weaponName == weaponName)
                return pose;
        }

        Debug.LogWarning($"WeaponPoseDatabase: {weaponName} 포즈가 존재하지 않음.");
        return null;
    }
    
}