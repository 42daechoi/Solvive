using UnityEngine.UI;
using UnityEngine;

public class SlotHighlight : MonoBehaviour
{
    public Image[] slotImages;
    public float defaultAlpha = 0.5f;
    public float highlightAlpha = 1.0f;

    public void UpdateSlotHighlight(int selectedSlot)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] != null)
            {
                Color color = slotImages[i].color;
                color.a = (i == selectedSlot) ? highlightAlpha : defaultAlpha;
                slotImages[i].color = color;
            }
        }
    }
}
