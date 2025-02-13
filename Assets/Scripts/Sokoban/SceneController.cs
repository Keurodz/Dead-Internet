using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// https://www.youtube.com/watch?v=HBEStd96UzI
// The SceneController class is responsible for managing the scene transitions.
// It triggers the transition animation and loads the next dungeon scene.
public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    [SerializeField] Animator transitionAnim;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Starts the transition animation.
    public void NextLevel() {
        StartCoroutine(LoadLevel());
    }

    // Triggers the transition animation and loads the next dungeon scene.
    IEnumerator LoadLevel() {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SokobanDungeonManager.Instance.NextDungeon();
        transitionAnim.SetTrigger("Start");
    }
}
