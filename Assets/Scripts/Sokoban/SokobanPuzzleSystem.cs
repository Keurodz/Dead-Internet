using System.Collections.Generic;
using UnityEngine;

/**
    This class manages the sokoban puzzle system, tracking the grid system and the level data.
**/
public class SokobanPuzzleSystem : MonoBehaviour
{
    public SokobanLevelData levelData;
    public SokobanGridSystem gridSystem;

    // the level controller for the puzzle system
    private ILevelController levelController;

    private bool hasWon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gridSystem = GetComponent<SokobanGridSystem>();
        levelController = DungeonSceneController.Instance;
        // this.LoadLevelData(this.levelData);
    }

    // Update is called once per frame
    private void Update()
    {
        if (CheckWinCondition() && !hasWon) {
            levelController.NextLevel();
            hasWon = true;
        }
    }

    // Loads the given level data into the puzzle system
    public void LoadLevelData(SokobanLevelData levelData)
    {
        this.levelData = levelData;
        if (gridSystem.PopulateGridWithBlocks(levelData)) {
            Debug.Log("Grid populated successfully");
        } else {
            Debug.Log("Grid population failed");
        }
    }

    // checks if the win condition is met
    private bool CheckWinCondition() {
        return gridSystem.CheckWinCondition();
    }
}
