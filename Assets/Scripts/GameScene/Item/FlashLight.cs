using UnityEngine;

[CreateAssetMenu(fileName = "Flashlight", menuName = "ScriptableObjects/Flashlight")]
public class Flashlight : Item
{
    public override void UseItem()
    {
        FlashLightControl flashLightControl = GetFlashLightControl();
        if (flashLightControl != null)
        {
            flashLightControl.ToggleFlashlight();
        }
        else
        {
            Debug.LogWarning("내 플레이어의 플래시라이트를 찾을 수 없습니다.");
        }
    }

    private FlashLightControl GetFlashLightControl()
    {
        if (PlayerController.Instance != null)
        {
            return PlayerController.Instance.GetComponentInChildren<FlashLightControl>();
        }
        return null;
    }
}
