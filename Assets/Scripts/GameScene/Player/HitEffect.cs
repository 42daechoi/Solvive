using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void OnHit()
    {
        skinnedMeshRenderer.material.DOColor(Color.red, "_BaseColor", 0.2f)
            .OnComplete(() => skinnedMeshRenderer.material.DOColor(Color.white, "_BaseColor", 0.5f));
    }
}
