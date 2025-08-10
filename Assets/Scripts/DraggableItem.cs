using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform correctZone;
    public float snapDistance = 0f; // jak blisko musi być do snapowania

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private GameObject closestZone = null;
    private GameObject usedZone = null;

    private bool inCorrectZone = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // żeby móc wykryć drop na inne obiekty
        originalScale = transform.localScale;
        originalPosition = transform.position;
        transform.localScale = originalScale * 1.2f;
        if (inCorrectZone) // Jeśli obiekt był w poprawnej strefie, to odejmij punkt
        {
            inCorrectZone = false;
            GameManager.Instance.SubstractPoint();
        }

        if (usedZone != null)
        {
            SnapZone snap = usedZone.GetComponent<SnapZone>();
            if (snap != null) // jeśli przedmiot był już w jakiejś strefie, to oznacz strefę jako nie zajętą
            {
                snap.isOccupied = false;
            }

        }
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
        GameObject[] snapZones = GameObject.FindGameObjectsWithTag("SnapZone"); // znajdź wszystkie obiekty z tagiem SnapZone

        closestZone = null;

        foreach (var zone in snapZones)
        {
            float dist = Vector2.Distance(transform.position, zone.transform.position);
            if (dist < snapDistance)
            {
                closestZone = zone; // znalezienie najbliższej wolnej strefy
            }
        }

        if (closestZone != null) 
        {
            SnapZone snap = closestZone.GetComponent<SnapZone>();
            if (!snap.isOccupied)
            {
                snap.isOccupied = true; // oznacz strefę jako zajętą
                Debug.Log("Is occupied");

                //transform.position = closestZone.transform.position; // Przenieś obiekt do strefy
                if (closestZone.transform == correctZone) //Sprawdzenie czy obiekt trafił do poprawnej strefy
                {
                    inCorrectZone = true;
                    GameManager.Instance.AddPoint(); // Dodaj punkt

                    transform.DOMove(closestZone.transform.position, 0.35f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            transform.DOScale(originalScale * 1.1f, 0.05f)
                                .OnComplete(() => transform.DOScale(originalScale, 0.1f));
                            transform.DORotate(new Vector3(0, 0, Random.Range(-5f, 5f)), 0.15f)
                                .OnComplete(() => transform.DORotate(Vector3.zero, 0.15f));
                        });
                }
                usedZone = closestZone; // Zapamiętaj strefę, w której jest obiekt
            }
            else
            {
                transform.DOMove(closestZone.transform.position, 0.3f)
                        .SetEase(Ease.OutQuad);
                //transform.position = originalPosition; // Jeśli strefa jest zajęta, przywróć oryginalną pozycję
            }

        }
    }
}
