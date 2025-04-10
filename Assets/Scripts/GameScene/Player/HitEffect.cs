using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Color originalColor;
    //private Color teamColor;
    
    
    public void SetOriginalColor(Color color)
    {
        originalColor = color;
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
            skinnedMeshRenderer.material.SetColor("_BaseColor", color);
    }
    
    /*public void SetTeamColor(Color color)
    {
        teamColor = color;
    }
    
    public void SetToTeamColor()
    {
        if (skinnedMeshRenderer != null)
            skinnedMeshRenderer.material.SetColor("_BaseColor", teamColor);
    }
    
    public void SetToOriginalColor()
    {
        if (skinnedMeshRenderer != null)
            skinnedMeshRenderer.material.SetColor("_BaseColor", originalColor);
    }*/

    public void OnHit()
    {
        if (skinnedMeshRenderer == null) return;

        Color currentBase = skinnedMeshRenderer.material.GetColor("_BaseColor");

        skinnedMeshRenderer.material.DOColor(Color.red, "_BaseColor", 0.2f)
            .OnComplete(() => skinnedMeshRenderer.material.DOColor(currentBase, "_BaseColor", 0.5f));
    }
}
