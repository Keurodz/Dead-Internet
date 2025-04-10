using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDisplayUI : MonoBehaviour
{
    private Text levelDisplayText;

    private ILevelProvider levelProvider;

    // attaches a listener to the SceneManager.sceneLoaded event to ensure 
    // the level text is updated when the scene loads
    private void Start() {
        levelProvider = SokobanDungeonManager.Instance;   
        levelDisplayText = GetComponentInChildren<Text>();
        UpdateLevelText();

        // SceneManager.sceneLoaded += OnSceneLoaded;
        levelProvider.OnLevelChanged += UpdateLevelText;
    }

    void OnDestroy()
    {
        levelProvider.OnLevelChanged -= UpdateLevelText;    
    }

    // updates the level text to display the current level

    private void UpdateLevelText()
    {
        // int currentLevel = SokobanDungeonManager.Instance.CurrentDungeonLevelIndex() + 1;
        // int totalLevels = SokobanDungeonManager.Instance.TotalDungeonLevels();
        int currentLevel = levelProvider.CurrentDungeonLevelIndex() + 1;
        int totalLevels = levelProvider.TotalDungeonLevels();
        levelDisplayText.text = $"Level: {currentLevel} / {totalLevels}";
    }
}
