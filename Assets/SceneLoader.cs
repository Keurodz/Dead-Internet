using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// VIDEO REFERENCE: https://www.youtube.com/watch?v=CE9VOZivb3I
public class SceneLoader : MonoBehaviour
{
    // name of the next scene to load
    [SerializeField] 
    private string nextSceneName;   
    // reference to the transition animator
    [SerializeField]
    private Animator transition;
    // the time it takes to transition to the next scene
    [SerializeField]
    private float transitionTime = 1f;

    void Start() {
        transition = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Loading next scene");
            LoadNextScene();
        }
    }

    // triggers the coroutine to load the next scene name
    private void LoadNextScene() {
        StartCoroutine(LoadScene(nextSceneName));
    }

    // coroutine to load the given scene with the animation
    IEnumerator LoadScene(string sceneName) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
