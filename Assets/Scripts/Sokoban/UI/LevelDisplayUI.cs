using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDisplayUI : MonoBehaviour
{
    private Text levelDisplayText;

    // attaches a listener to the SceneManager.sceneLoaded event to ensure 
    // the level text is updated when the scene loads
    void Start()
    {
        levelDisplayText = GetComponentInChildren<Text>();

        UpdateLevelText();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateLevelText();
    }

    // updates the level text to display the current level

    private void UpdateLevelText()
    {
        int currentLevel = SokobanDungeonManager.Instance.CurrentDungeonLevelIndex() + 1;
        int totalLevels = SokobanDungeonManager.Instance.TotalDungeonLevels();
        levelDisplayText.text = $"Level: {currentLevel} / {totalLevels}";
    }
}
