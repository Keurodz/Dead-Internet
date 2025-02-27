using System.Collections.Generic;
using UnityEngine;

/**
    This class manages the sokoban puzzle system, tracking the grid system and the level data.
**/
public class SokobanPuzzleSystem : MonoBehaviour
{
    public SokobanLevelData levelData;
    public SokobanGridSystem gridSystem;

    private bool hasWon = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridSystem = GetComponent<SokobanGridSystem>();
        if (gridSystem.PopulateGridWithBlocks(levelData)) {
            Debug.Log("Grid populated successfully");
        } else {
            Debug.Log("Grid population failed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckWinCondition() && !hasWon) {
            SceneController.Instance.NextLevel();
            hasWon = true;
        }
    }

    // checks if the win condition is met
    private bool CheckWinCondition() {
        return gridSystem.CheckWinCondition();
    }
}
