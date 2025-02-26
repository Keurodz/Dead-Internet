using UnityEngine;
using UnityEngine.UI;

public class LevelDisplayUI : MonoBehaviour
{
    private Text levelDisplayText;

    void Start()
    {
        levelDisplayText = GetComponentInChildren<Text>();
        levelDisplayText.text = "Level: " + (SokobanDungeonManager.Instance.currentDungeonLevelIndex() + 1) + " / " + SokobanDungeonManager.Instance.totalDungeonLevels();
    }
}
