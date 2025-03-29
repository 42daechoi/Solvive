using UnityEngine;

[CreateAssetMenu(fileName = "Flashlight", menuName = "ScriptableObjects/Flashlight")]
public class Flashlight : ItemData
{
    public override void UseItem()
    {
        FlashLightControl flashLightControl = GetFlashLightControl();
        FlashlightSound flashlightSound = GetFlashlightSound();
        if (flashLightControl != null)
        {
            flashLightControl.ToggleFlashlight();
            if (flashlightSound != null)
            {
                flashlightSound.PlayClickSound();
            }
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

    private FlashlightSound GetFlashlightSound()
    {
        if (PlayerController.Instance != null)
        {
            return PlayerController.Instance.GetComponentInChildren<FlashlightSound>();
        }
        return null;
    }
}
