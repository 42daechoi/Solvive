using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    // 싱글턴 인스턴스
    public static BrightnessController Instance { get; private set; }

    public Volume postProcessVolume;
    public Slider brightnessSlider;
    private Exposure exposure;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out exposure))
        {
            exposure.compensation.value = 0f;
        }
        else
        {
            Debug.LogError("Exposure Override를 찾을 수 없습니다.");
        }
    }
    
    public void SetBrightness(float value)
    {
        if (exposure != null)
        {
            exposure.compensation.value = value;
        }
    }
}