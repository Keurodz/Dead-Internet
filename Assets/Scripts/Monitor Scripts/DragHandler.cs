using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform canvasRect;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasRect = canvas.GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Bring this object to the top of the hierarchy
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition = ClampToCanvas(newPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private Vector2 ClampToCanvas(Vector2 position)
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetLocalCorners(canvasCorners);

        float minX = canvasCorners[0].x + rectTransform.rect.width * rectTransform.pivot.x;
        float maxX = canvasCorners[2].x - rectTransform.rect.width * (1 - rectTransform.pivot.x);
        float minY = canvasCorners[0].y + rectTransform.rect.height * rectTransform.pivot.y;
        float maxY = canvasCorners[2].y - rectTransform.rect.height * (1 - rectTransform.pivot.y);

        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}
