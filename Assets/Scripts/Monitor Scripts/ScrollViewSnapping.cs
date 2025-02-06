using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSnapping : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float snapSpeed = 5f;

    void Update()
    {
        if (scrollRect.verticalNormalizedPosition < 0f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0f, Time.deltaTime * snapSpeed);
        }
        else if (scrollRect.verticalNormalizedPosition > 1f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 1f, Time.deltaTime * snapSpeed);
        }
    }
}
