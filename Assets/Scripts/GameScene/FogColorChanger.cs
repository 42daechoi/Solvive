using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;

public class FogColorChanger : MonoBehaviour
{
    private Volume skyAndFogVolume;
    private Fog fog;

    private void Start()
    {
        if (skyAndFogVolume == null)
        {
            skyAndFogVolume = GetComponent<Volume>();
        }
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOneCitizenAlive += ChangeFogColor;
        }
    }

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnOneCitizenAlive -= ChangeFogColor;
        }
    }

    public void ChangeFogColor()
    {
        if (skyAndFogVolume.profile.TryGet<Fog>(out fog))
        {
            Color targetColor = new Color(1.0f, 0.572f, 0.596f);
            StartCoroutine(ChangeFogColorOverTime(targetColor, 5f));
        }
        else
        {
            Debug.Log("FogColorChanger : Fog가 null입니다.");
        }
    }

    public IEnumerator ChangeFogColorOverTime(Color targetColor, float duration)
    {
        if (fog == null) yield break;

        skyAndFogVolume.weight = 1f;
        fog.albedo.overrideState = true;

        Color startColor = fog.albedo.value;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            fog.albedo.value = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        fog.albedo.value = targetColor;
    }
}