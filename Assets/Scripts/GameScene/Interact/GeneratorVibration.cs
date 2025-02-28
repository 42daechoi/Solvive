using UnityEngine;

public class GeneratorVibration : MonoBehaviour
{

    [SerializeField] private float vibrationIntensity = 0.01f;
    [SerializeField] private float vibrationSpeed = 20f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isGeneratorActive = false;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (isGeneratorActive)
        {
            float x = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float y = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;

            transform.position = originalPosition + new Vector3(x, y, 0);
            transform.rotation = originalRotation * Quaternion.Euler(x * 2f, y * 2f, 0);
        }
        else
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }

    public void SetIsGeneratorActive(bool _isGeneratorActive)
    {
        isGeneratorActive = _isGeneratorActive;
    }

}
