using UnityEngine;
using System.Collections;

public class SokobanLevelManager : MonoBehaviour
{
    public GameObject winCanvas;

    private CanvasGroup winCanvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winCanvas.SetActive(false);
        winCanvasGroup = winCanvas.GetComponent<CanvasGroup>();
    }

    public void OnWin()
    {
        Debug.Log("You win!");
        winCanvas.SetActive(true);

        StartCoroutine(FadeInCanvas());
    }

    private IEnumerator FadeInCanvas()
    {
        float duration = 3.0f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            winCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        winCanvasGroup.alpha = 1f; 
    }
}
