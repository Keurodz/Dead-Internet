using DG.Tweening;
using UnityEngine;

public class PanelAnimator : MonoBehaviour
{
    public float fadeDuration = 0.3f; // Duration of the fade animation
    public float scaleDuration = 0.3f; // Duration of the scale animation
    public Ease fadeEase = Ease.OutQuad; // Easing for the fade
    public Ease scaleEase = Ease.OutBack; // Easing for the scale

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        // Get references to the CanvasGroup and RectTransform
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Ensure the panel starts closed
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.zero;
        }
    }

    public void OpenPanel()
    {
        // Enable the panel (if it was disabled)
        gameObject.SetActive(true);

        // Fade in
        canvasGroup.DOFade(1, fadeDuration)
                   .SetEase(fadeEase);

        // Scale up
        rectTransform.DOScale(Vector3.one, scaleDuration)
                     .SetEase(scaleEase);
    }

    public void ClosePanel()
    {
        // Fade out
        canvasGroup.DOFade(0, fadeDuration)
                   .SetEase(fadeEase);

        // Scale down
        rectTransform.DOScale(Vector3.zero, scaleDuration)
                     .SetEase(scaleEase)
                     .OnComplete(() => gameObject.SetActive(false)); // Disable the panel after animation
    }
}