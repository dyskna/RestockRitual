using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;

    public float snapDistance = 0f; // jak blisko musi być do snapowania
    private Vector3 originalScale;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // żeby móc wykryć drop na inne obiekty
        originalScale = transform.localScale;         
        transform.localScale = originalScale * 1.2f;
    }

    public void OnDrag(PointerEventData eventData)
{
    Vector3 screenPoint = Input.mousePosition;
    screenPoint.z = 10f; // odległość od kamery
    transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
}


    public void OnEndDrag(PointerEventData eventData)
{  
    canvasGroup.blocksRaycasts = true;
    transform.localScale = originalScale; 
    GameObject[] snapZones = GameObject.FindGameObjectsWithTag("SnapZone");

    //float closestDistance = Mathf.Infinity;
    Transform closestZone = null;

    foreach (var zone in snapZones)
    {
        float dist = Vector2.Distance(transform.position, zone.transform.position);
            Debug.Log(dist);
            Debug.Log(snapDistance);
            if (dist < snapDistance)
            {
                //closestDistance = dist;
                closestZone = zone.transform;
                Debug.Log("????");
            }
    }

    if (closestZone != null)
    {
        transform.position = closestZone.position;
        closestZone = null;
    }

}

}
