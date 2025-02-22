using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueAnimator : MonoBehaviour
{
    public static void AnimateFadeIn(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.4f);
        }
    }

    public static void AnimateCommentSlideIn(RectTransform commentRect)
    {
        // Ensure the object starts off-screen (adjust X position)
        Vector2 startPosition = commentRect.anchoredPosition;
        commentRect.anchoredPosition = new Vector2(startPosition.x + 1000, startPosition.y);

        // Slide in with bounce effect
        commentRect.DOAnchorPosX(startPosition.x, 0.5f)
            .SetEase(Ease.OutBack);
    }

    public static void AnimateFadeOut(GameObject obj)
    {
        if (obj != null)
        {
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
            canvasGroup.DOFade(0f, 0.3f).OnComplete(() => obj.SetActive(false));
        }
    }

    public static void AnimateScaleIn(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }
    }

    public static void AnimateScaleOut(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => obj.SetActive(false));
        }
    }

    public static void AnimateSlideIn(GameObject obj, float offset = 100f)
    {
        if (obj != null)
        {
            // Ensure object is active before animation
            obj.SetActive(true);
            RectTransform rect = obj.GetComponent<RectTransform>();

            // Kill any existing tweens to prevent conflicts
            DOTween.Kill(rect);

            // Store original position
            Vector2 originalPosition = rect.anchoredPosition;
            // Set starting position offset to the right
            rect.anchoredPosition = originalPosition + new Vector2(offset, 0);
            // Animate back to original position
            rect.DOAnchorPos(originalPosition, 0.5f).SetEase(Ease.OutExpo);
        }
    }

    public static void AnimateSlideOut(GameObject obj, float toX = 1000f)
    {
        if (obj != null && obj.activeInHierarchy)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();

            // Kill any existing tweens to prevent conflicts
            DOTween.Kill(rect);

            // Store original position
            Vector2 startPosition = rect.anchoredPosition;

            // Create a sequence to handle the animation and deactivation
            Sequence sequence = DOTween.Sequence();

            // Add the slide animation
            sequence.Append(rect.DOAnchorPosX(startPosition.x + toX, 0.4f).SetEase(Ease.InExpo));

            // Only deactivate after the animation is complete
            sequence.OnComplete(() => {
                if (obj != null) // Check again in case object was destroyed during animation
                {
                    obj.SetActive(false);
                    // Reset position to original
                    rect.anchoredPosition = startPosition;
                }
            });
        }
    }
}
