using Photon.Pun;
using UnityEngine.UI;
using UnityEngine;

public class MicUI : MonoBehaviourPun
{
    public Image micImage;
    public Sprite onSprite;
    public Sprite offSprite;
    private int micValue;

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
        micValue = VolumeSittings.Instance.micMode;
        if (flag)
        {
            micImage.sprite = onSprite;
        }
        else if (micValue == 1)
        {
            micImage.sprite = onSprite;
        }
        else
        {
            micImage.sprite = offSprite;
        }
    }
}
