using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public RectTransform parentPanel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Find the parent panel (the first parent with RectTransform)
        //Transform parent = transform.parent;
        //if (parent != null)
        //{
        //    parentPanel = parent.GetComponent<RectTransform>();
        //}

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Bring this object to the top of the hierarchy within its parent
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen position to local position within parent
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentPanel,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        // Calculate new position
        rectTransform.localPosition = ClampToParentPanel(localPoint);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private Vector2 ClampToParentPanel(Vector2 position)
    {
        if (parentPanel == null)
            return position;

        // Get the corners of the parent panel
        Vector3[] parentCorners = new Vector3[4];
        parentPanel.GetLocalCorners(parentCorners);

        // Calculate the boundaries based on the parent's corners and this object's size
        float minX = parentCorners[0].x + rectTransform.rect.width * rectTransform.pivot.x;
        float maxX = parentCorners[2].x - rectTransform.rect.width * (1 - rectTransform.pivot.x);
        float minY = parentCorners[0].y + rectTransform.rect.height * rectTransform.pivot.y;
        float maxY = parentCorners[2].y - rectTransform.rect.height * (1 - rectTransform.pivot.y);

        // Clamp the position within the boundaries
        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}