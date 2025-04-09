using UnityEngine.UI;
using UnityEngine;

public class MicUI : MonoBehaviour
{
    public Image micImage;
    public Sprite onSprite;
    public Sprite offSprite;

    private void Start()
    {
        EventManager_Game.Instance.OnVoice += HandleMicUI;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnVoice -= HandleMicUI;
    }



    private void HandleMicUI(bool flag)
    {
        if (flag)
        {
            micImage.sprite = onSprite;
        }
        else
        {
            micImage.sprite = offSprite;
        }
    }
}
