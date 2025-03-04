using System;
using UnityEngine;

public class SlotHighlight : MonoBehaviour
{
    public Transform[] slotTransforms;
    public Vector3 defaultScale = Vector3.one;
    public Vector3 highlightScale = Vector3.one * 1.2f;

    private void Awake()
    {
        slotTransforms = new Transform[5];
        
        GameObject slotContainer = GameObject.Find("SlotContainer");
        if (slotContainer != null)
        {
            slotTransforms[0] = slotContainer.transform.Find("Slot01");
            slotTransforms[1] = slotContainer.transform.Find("Slot02");
            slotTransforms[2] = slotContainer.transform.Find("Slot03");
            slotTransforms[3] = slotContainer.transform.Find("Slot04");
            slotTransforms[4] = slotContainer.transform.Find("Slot05");
        }
        else
        {
            Debug.LogError("SlotContainer가 업씀");
        }
    }

    private void Start()
    {
        
    }

    public void UpdateSlotHighlight(int selectedSlot)
    {
        for (int i = 0; i < slotTransforms.Length; i++)
        {
            if (slotTransforms[i] != null)
            {
                slotTransforms[i].localScale = (i == selectedSlot)
                    ? highlightScale
                    : defaultScale;
            }
        }
    }
}