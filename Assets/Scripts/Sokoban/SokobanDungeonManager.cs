using UnityEngine;
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
public class SokobanDungeonManager : MonoBehaviour
{
    public static SokobanDungeonManager Instance;

    // the list of dungeon level scenes
    [SerializeField] 
    public List<string> dungeonLevelScenes;
    // the scene to load after all dungeon levels are completed
    [SerializeField]
    public string nextSceneName;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private int currentSceneIndex = 0;

    // Loads the first dungeon level.
    public void EnterDungeon() {
        currentSceneIndex = 0;
        SceneManager.LoadScene(dungeonLevelScenes[currentSceneIndex]);
    }

    // Progresses the index to the next dungeon level index.
    public void NextDungeon() {
        if (currentSceneIndex < dungeonLevelScenes.Count - 1) {
            Debug.Log("Loading next dungeon level");
            currentSceneIndex++;
            SceneManager.LoadSceneAsync(dungeonLevelScenes[currentSceneIndex]);
        } else {
            // no more dungeon levels left so goes to next scene 
            Debug.Log("Loading next scene");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
