using UnityEngine;
using UnityEngine.UI;

/*
 * Entire purpose of this class is to set the scrollbar to the top instead of the default bottom
 */
public class ScrollBarSetter : MonoBehaviour
{
    private ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        // Ensure it's set at the top after UI updates
        StartCoroutine(SetScrollToTop());
    }

    private System.Collections.IEnumerator SetScrollToTop()
    {
        /// Wait for the UI to update fully before setting position
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame(); // Extra frame for safety

        if (scrollRect != null)
        {
            // Force a layout rebuild to ensure correct positioning
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

            // Ensure it's at the top
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

}
