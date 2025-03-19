using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TakeDamageUI : MonoBehaviour
{
    [SerializeField] private Image takeDamageImage;
    [SerializeField] private float fadeDuration = 2f;

    private void OnEnable()
    {
        EventManager_Game.Instance.OnTakeDamage += ActiveTakeDamageUI;
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnTakeDamage -= ActiveTakeDamageUI;
    }



    private Color hitColor = new Color(63f / 255f, 13f / 255f, 13f / 255f, 130f / 255f);

    private void Start()
    {
        if (takeDamageImage != null)
        {
            takeDamageImage.color = new Color(1, 0, 0, 0);
        }
    }

    public void ActiveTakeDamageUI()
    {
        StartCoroutine(PlayHitEffect());
    }

    private IEnumerator PlayHitEffect()
    {
        takeDamageImage.color = hitColor;


        yield return new WaitForSeconds(0.2f);


        float timeElapsed = 0;
        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(hitColor.a, 0, timeElapsed / fadeDuration);
            takeDamageImage.color = new Color(hitColor.r, hitColor.g, hitColor.b, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }


        takeDamageImage.color = new Color(hitColor.r, hitColor.g, hitColor.b, 0);
    }

}
