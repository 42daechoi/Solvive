using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EndGameButtonTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    public TextMeshProUGUI tmp;
    public Sprite normalSprite;
    public Sprite highlightedSprite;

    private void Start()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }
        if (tmp == null)
        {
            tmp = GetComponentInChildren<TextMeshProUGUI>();
        }

        SetImageTransparency(0f);
        buttonImage.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetImageTransparency(1f);
        tmp.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetImageTransparency(0f);
        tmp.color = Color.white;
    }

    private void SetImageTransparency(float alpha)
    {
        Color color = buttonImage.color;
        color.a = alpha;
        buttonImage.color = color;
    }
}
