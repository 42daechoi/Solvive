using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InventoryUI : MonoBehaviourPun
{
    public static InventoryUI Instance { get; private set; }

    public Image[] slotUI;

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

    public void UpdateUI(Inventory inventory)
    {
        Item[] itemSlots = inventory.GetItemSlots();
        for (int i = 1; i < slotUI.Length; i++)
        {
            if (i < itemSlots.Length && itemSlots[i - 1] != null)
            {
                slotUI[i].sprite = itemSlots[i - 1].icon;
            }
            else
            {
                slotUI[i].sprite = null;
            }
        }
    }
}
