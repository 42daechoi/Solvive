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
    public abstract void UseItem();
}