using DG.Tweening;
using UnityEngine;
public class SmoothPanel : MonoBehaviour
{
    [Header("Panel Animation")]
    public UIAnimationSettings showAnimation = new UIAnimationSettings { duration = 0.4f, easeType = Ease.OutBack };
    public UIAnimationSettings hideAnimation = new UIAnimationSettings { duration = 0.3f, easeType = Ease.InQuad };
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
        
        // Start invisible and small
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.8f;
        
        // Animate in
        Sequence showSequence = DOTween.Sequence();
        showSequence.Append(canvasGroup.DOFade(1f, showAnimation.duration));
        showSequence.Join(rectTransform.DOScale(Vector3.one, showAnimation.duration).SetEase(showAnimation.easeType));
    }
    
    public void Hide(System.Action onComplete = null)
    {
        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Append(canvasGroup.DOFade(0f, hideAnimation.duration));
        hideSequence.Join(rectTransform.DOScale(Vector3.one * 0.8f, hideAnimation.duration).SetEase(hideAnimation.easeType));
        hideSequence.OnComplete(() => 
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}