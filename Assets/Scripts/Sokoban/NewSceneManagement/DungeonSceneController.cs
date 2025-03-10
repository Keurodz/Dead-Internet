using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// The DungeonSceneController class is used to manage the dungeon levels within a single scene. 
// It is an update of the SokobanDungeonManager class, which was used to manage the dungeon levels across multiple scenes.
public class DungeonSceneController : MonoBehaviour, ILevelProvider, ILevelController
{
    public static DungeonSceneController Instance;

    // event that is triggered when the level changes
    public event Action OnLevelChanged = delegate { }; 

    // the player character
    [SerializeField]
    private GameObject player;

    // reference to the animator for the transition animation
    [SerializeField] 
    Animator transitionAnim;

    // the list of dungeon level scenes
    [SerializeField] 
    private List<SokobanLevelData> levelDataList;
    // the scene to load after all dungeon levels are completed
    [SerializeField]
    private string nextSceneName;

    // reference to the puzzle system
    [SerializeField]
    private SokobanPuzzleSystem puzzleSystem; 

    // the player's initial position 
    private Vector3 playerInitialPosition;

    // the dungeon environments 
    private Dictionary<string, GameObject> dungeonEnvironments = new Dictionary<string, GameObject>();

    // the current dungeon environment
    private GameObject currentEnvironment;

    // the index of the current dungeon level
    private int currentSceneIndex = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            Debug.Log("DungeonSceneController created");
            // DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public int CurrentDungeonLevelIndex() => currentSceneIndex;
    public int TotalDungeonLevels() => levelDataList.Count;

    private void Start() {
        playerInitialPosition = player.transform.position;
        LoadLevelData(levelDataList[currentSceneIndex]);
    }

    // Progresses the index to the next dungeon level index.
    public void NextLevel() {
        if (currentSceneIndex < levelDataList.Count - 1) {
            Debug.LogWarning("LOADING NEXT LEVEL");
            currentSceneIndex++;

            StartCoroutine(LoadLevel(levelDataList[currentSceneIndex]));
            OnLevelChanged?.Invoke(); 
            // SceneManager.LoadSceneAsync(dungeonLevelScenes[currentSceneIndex]);
        } else {
            Debug.LogWarning("LOADING NEXT SCENE");

            // no more dungeon levels left so goes to next scene 
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // Restarts the current dungeon level
    public void RestartLevel() {
        StartCoroutine(LoadLevel(levelDataList[currentSceneIndex]));
    }

    // loads the sokoban environments from the resources 
    private void LoadDungeonEnvironments() {
        GameObject[] environments = Resources.LoadAll<GameObject>("Sokoban/Environments");
        foreach (GameObject environment in environments) {
            dungeonEnvironments.Add(environment.name, environment);
        }
    }

    // loads the given level data into the puzzle system
    private void LoadLevelData(SokobanLevelData levelData) {
        Debug.Log("Loading level data: " + levelData.name);
        player.transform.position = playerInitialPosition;
        puzzleSystem.LoadLevelData(levelData);
        LoadEnvironment(levelData);
    }

    // loads the environment for the given level data
    private void LoadEnvironment(SokobanLevelData levelData) {
        string environmentName = levelData.GetEnvironmentName();
        if (dungeonEnvironments.Count == 0) {
            LoadDungeonEnvironments();
        }

        if (dungeonEnvironments.ContainsKey(environmentName)) {
            GameObject environment = dungeonEnvironments[environmentName];
            if (currentEnvironment != null) {
                Destroy(currentEnvironment);
            }
            currentEnvironment = Instantiate(environment);
        } else {
            Debug.LogWarning("Environment not found: " + environmentName);
        }
    }

    // loads the given level data, triggers the transition animation
    private IEnumerator LoadLevel(SokobanLevelData levelData) {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        LoadLevelData(levelData);
        transitionAnim.SetTrigger("Start");
    }
}
