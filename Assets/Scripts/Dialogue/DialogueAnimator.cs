using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogueAnimator : MonoBehaviour
{
    public static void AnimateCommentSlideIn(RectTransform commentRect)
    {
        // Ensure the object starts off-screen (adjust X position)
        Vector2 startPosition = commentRect.anchoredPosition;
        commentRect.anchoredPosition = new Vector2(startPosition.x + 1000, startPosition.y);

        // Slide in with bounce effect
        commentRect.DOAnchorPosX(startPosition.x, 0.5f)
            .SetEase(Ease.OutBack);
    }
}
