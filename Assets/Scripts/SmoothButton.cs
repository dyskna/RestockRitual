using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SmoothButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    public UIAnimationSettings pressAnimation = new UIAnimationSettings { duration = 0.1f };
    public UIAnimationSettings hoverAnimation = new UIAnimationSettings { duration = 0.2f };
    
    [Header("Scale Settings")]
    public Vector3 pressedScale = Vector3.one * 0.95f;
    public Vector3 hoveredScale = Vector3.one * 1.05f;
    
    private Vector3 originalScale;
    private RectTransform rectTransform;
    private Button button;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        originalScale = rectTransform.localScale;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        
        rectTransform.DOScale(pressedScale, pressAnimation.duration)
            .SetEase(pressAnimation.easeType)
            .SetUpdate(pressAnimation.useUnscaledTime);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        
        rectTransform.DOScale(originalScale, pressAnimation.duration)
            .SetEase(pressAnimation.easeType)
            .SetUpdate(pressAnimation.useUnscaledTime);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        
        rectTransform.DOScale(hoveredScale, hoverAnimation.duration)
            .SetEase(hoverAnimation.easeType)
            .SetUpdate(hoverAnimation.useUnscaledTime);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(originalScale, hoverAnimation.duration)
            .SetEase(hoverAnimation.easeType)
            .SetUpdate(hoverAnimation.useUnscaledTime);
    }
    
    void OnDestroy()
    {
        rectTransform?.DOKill();
    }
}