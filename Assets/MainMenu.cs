using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.Playables;

// Video Reference:
// https://www.youtube.com/watch?v=_jtj73lu2Ko
public class MainMenu : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement root;

    private Button startButton;
    private Button quitButton;
    private PlayableDirector startPlayableDirector;

    private void Awake() {
        _document = GetComponent<UIDocument>();
        root = _document.rootVisualElement;

        startButton = _document.rootVisualElement.Q<Button>("PlayButton") as Button;
        startButton.clicked += OnStartButtonClicked;

        quitButton = _document.rootVisualElement.Q<Button>("QuitButton") as Button;
        quitButton.clicked += OnQuitButtonClicked;

        startPlayableDirector = GameObject.Find("Monitor Screen").GetComponent<PlayableDirector>();
    }

    private void OnStartButtonClicked() {
        Debug.Log("Start Button Clicked");
        StartCoroutine(AnimateMainMenuOffScreen());
    }

    private IEnumerator AnimateMainMenuOffScreen() {
        var initialPosition = root.resolvedStyle.left;
        startPlayableDirector.Play();

       
        float duration = 5.0f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float newPos = Mathf.Lerp(initialPosition, -root.resolvedStyle.width, elapsedTime / duration);
            root.style.left = newPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        root.style.left = -root.resolvedStyle.width;
        this.gameObject.SetActive(false);
    }

    private void OnQuitButtonClicked() {
        Application.Quit();
    }

    private void OnDestroy() {
        startButton.clicked -= OnStartButtonClicked;
        quitButton.clicked -= OnQuitButtonClicked;
    }
}
