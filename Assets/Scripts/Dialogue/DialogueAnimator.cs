using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueAnimator : MonoBehaviour
{
    // Fades in a UI object using CanvasGroup
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

    // Slides a comment in from the right with a bounce
    public static void AnimateCommentSlideIn(RectTransform commentRect)
    {
        Vector2 startPosition = commentRect.anchoredPosition;
        commentRect.anchoredPosition = new Vector2(startPosition.x + 1000, startPosition.y);
        commentRect.DOAnchorPosX(startPosition.x, 0.5f).SetEase(Ease.OutBack);
    }

    // Fades out a UI object and deactivates it
    public static void AnimateFadeOut(GameObject obj)
    {
        if (obj != null)
        {
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>() ?? obj.AddComponent<CanvasGroup>();
            canvasGroup.DOFade(0f, 0.3f).OnComplete(() => obj.SetActive(false));
        }
    }

    // Scales a UI object up from zero with easing
    public static void AnimateScaleIn(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }
    }

    // Scales a UI object down to zero and deactivates it
    public static void AnimateScaleOut(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => obj.SetActive(false));
        }
    }

    // Slides a UI object in from the right by a given offset
    public static void AnimateSlideIn(GameObject obj, float offset = 100f)
    {
        if (obj != null)
        {
            obj.SetActive(true);
            RectTransform rect = obj.GetComponent<RectTransform>();
            DOTween.Kill(rect); // Stop any previous tweens

            Vector2 originalPosition = rect.anchoredPosition;
            rect.anchoredPosition = originalPosition + new Vector2(offset, 0);
            rect.DOAnchorPos(originalPosition, 0.5f).SetEase(Ease.OutExpo);
        }
    }

    // Slides a UI object out to the right and deactivates it after
    public static void AnimateSlideOut(GameObject obj, float toX = 1000f)
    {
        if (obj != null && obj.activeInHierarchy)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            DOTween.Kill(rect); // Stop any previous tweens

            Vector2 startPosition = rect.anchoredPosition;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(rect.DOAnchorPosX(startPosition.x + toX, 0.4f).SetEase(Ease.InExpo));
            sequence.OnComplete(() =>
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    rect.anchoredPosition = startPosition; // Reset position
                }
            });
        }
    }
}
