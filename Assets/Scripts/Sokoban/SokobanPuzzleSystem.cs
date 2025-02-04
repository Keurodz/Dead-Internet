using System.Collections.Generic;
using UnityEngine;

/**
    This class manages the sokoban puzzle system, tracking the grid system and the level data.
**/
public class SokobanPuzzleSystem : MonoBehaviour
{
    public SokobanLevelData levelData;
    public SokobanGridSystem gridSystem;

    // the level manager
    private SokobanLevelManager levelManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridSystem = GetComponent<SokobanGridSystem>();
        if (gridSystem.PopulateGridWithBlocks(levelData)) {
            Debug.Log("Grid populated successfully");
        } else {
            Debug.Log("Grid population failed");
        }

        levelManager = FindAnyObjectByType<SokobanLevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckWinCondition()) {
            levelManager.OnWin();
        }
    }

    // checks if the win condition is met
    private bool CheckWinCondition() {
        return gridSystem.CheckWinCondition();
    }
}
