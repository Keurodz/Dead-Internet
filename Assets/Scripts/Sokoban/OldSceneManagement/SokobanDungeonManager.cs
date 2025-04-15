using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// The SokobanDungeonManager class is responsible for managing the dungeon levels.
// It configures the dungeon by providing all the dungeon level scene names 
// and the scene to load after all dungeon levels are completed.
// HOW TO USE: Attach this script to an empty GameObject in the scene, 
// ideally the GameObject that will trigger the dungeon entering. 
// Set the dungeonLevelScenes and nextSceneName fields in the Inspector.
// Call the EnterDungeon method to start the dungeon.
public class SokobanDungeonManager : MonoBehaviour, ILevelProvider, ILevelController
{
    public static SokobanDungeonManager Instance;

    // event that is triggered when the level changes
    public event Action OnLevelChanged = delegate { }; 

    // the list of dungeon level scenes
    [SerializeField] 
    public List<string> dungeonLevelScenes;
    // the scene to load after all dungeon levels are completed
    [SerializeField]
    public string nextSceneName;

    // the index of the current dungeon level
    private int currentSceneIndex = 0;

    [Header("Controls the animation of the transition")]
    [SerializeField]
    private Animator transitionAnim;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (transitionAnim == null) {
            transitionAnim = GetComponentInChildren<Animator>();
        }
    }

    // returns the current dungeon level index
    public int CurrentDungeonLevelIndex() {
        return currentSceneIndex;
    }

    // returns the total number of dungeon levels
    public int TotalDungeonLevels() {
        return dungeonLevelScenes.Count;
    }

    // Loads the first dungeon level.
    public void EnterDungeon() {
        currentSceneIndex = 0;
        SceneManager.LoadScene(dungeonLevelScenes[currentSceneIndex]);
    }

    // Progresses the index to the next dungeon level index.
    public void NextLevel() {
        if (currentSceneIndex < dungeonLevelScenes.Count - 1) {
            currentSceneIndex++;
            StartCoroutine(LoadNextLevel());
        } else {
            // no more dungeon levels left so goes to next scene 
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void PlayWinSequence() {

    }

    public void RestartLevel() {
        SceneManager.LoadScene(dungeonLevelScenes[currentSceneIndex]);
    }

    // Triggers the transition animation and loads the next dungeon scene.
    private IEnumerator LoadNextLevel() {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(dungeonLevelScenes[currentSceneIndex]);
        OnLevelChanged?.Invoke();
        transitionAnim.SetTrigger("Start");
    }
}
