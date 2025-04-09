using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Color originalColor;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetHitEffect(this);
        }
    }
    
    public void SetOriginalColor(Color color)
    {
        originalColor = color;
        if (skinnedMeshRenderer != null)
            skinnedMeshRenderer.material.SetColor("_BaseColor", color);
    }

    public void OnHit()
    {
        skinnedMeshRenderer.material.DOColor(Color.red, "_BaseColor", 0.2f)
            .OnComplete(() => skinnedMeshRenderer.material.DOColor(originalColor, "_BaseColor", 0.5f));
    }
}
