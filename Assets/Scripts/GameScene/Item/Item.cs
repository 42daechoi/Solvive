using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Farming/Item")]
public abstract class Item : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string itemName;
    public Sprite icon;

    [Header("Equip Settings")]
    public Vector3 equipPosition;
    public Vector3 equipRotation;
    
    [Header("IK Settings")]
    public Vector3 rightHandIKPosition;
    public Vector3 rightHandIKRotation;
    public Vector3 leftHandIKPosition;
    public Vector3 leftHandIKRotation;
    public abstract void UseItem();
}