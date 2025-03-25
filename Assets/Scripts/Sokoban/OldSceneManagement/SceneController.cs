using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// https://www.youtube.com/watch?v=HBEStd96UzI
// The SceneController class is responsible for managing the scene transitions.
// It triggers the transition animation and loads the next dungeon scene.
public class SceneController : MonoBehaviour, ILevelController
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

    // Starts the transition animation as the win sequence.
    public void NextLevel() {
        StartCoroutine(LoadLevel());
    }

    // starts the transition animation as the win sequence.
    public void PlayWinSequence() {
        StartCoroutine(LoadLevel());
    }

    // Restarts the current dungeon scene.
    public void RestartLevel() {
        StartCoroutine(Restart());
    }

    // Triggers the transition animation and reloads the current dungeon scene.
    private IEnumerator Restart() {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        Debug.Log("Restarting level " + SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        transitionAnim.SetTrigger("Start");
    }

    // Triggers the transition animation and loads the next dungeon scene.
    private IEnumerator LoadLevel() {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SokobanDungeonManager.Instance.NextDungeon();
        transitionAnim.SetTrigger("Start");
    }
}
