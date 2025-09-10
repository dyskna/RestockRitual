using DG.Tweening;
using UnityEngine;
[System.Serializable]
public class UIAnimationSettings
{
    public float duration = 0.3f;
    public Ease easeType = Ease.OutQuad;
    public bool useUnscaledTime = false;
}