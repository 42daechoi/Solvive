using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] private ItemData[] items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemByName(string itemName)
    {
        foreach (ItemData item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        Debug.LogError($"ItemManager : 아이템 {itemName}을 찾을 수 없습니다.");
        return null;
    }
}
