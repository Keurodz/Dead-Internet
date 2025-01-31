using System.Collections.Generic;
using UnityEngine;

public class SokobanPuzzleSystem : MonoBehaviour
{
    public SokobanLevelData levelData;
    public SokobanGridSystem gridSystem;

    public GameObject movablePrefab;
    public GameObject immovablePrefab;
    public GameObject buttonPrefab;

    // the positions where buttons are located
    private List<Vector2Int> winPositions = new List<Vector2Int>();
    // the win canvas
    private SokobanLevelManager levelManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridSystem = GetComponent<SokobanGridSystem>();
        LoadLevelData();
        gridSystem.PopulateGridWithBlocks();

        levelManager = FindAnyObjectByType<SokobanLevelManager>();
    }

    // uses the level data to populate the sokoban level
    void LoadLevelData() {
        for (int i = 0; i < levelData.interactableObjects.Length; i++)
        {
            InteractableObject interactable = levelData.interactableObjects[i];
            Vector3 worldPosition = gridSystem.GetWorldPosition(interactable.gridPosition);

            GameObject prefab = (interactable.type == InteractableObjectType.MovableBlockObject) ? movablePrefab : 
            (interactable.type == InteractableObjectType.ImmovableBlockObject) ? immovablePrefab : 
            (interactable.type == InteractableObjectType.ButtonBlockObject) ? buttonPrefab : null;

            if (interactable.type == InteractableObjectType.ButtonBlockObject) {
                winPositions.Add(interactable.gridPosition);
            }

            GameObject interactableGameObject = Instantiate(prefab, worldPosition, Quaternion.identity);
        }
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
        foreach (Vector2Int winPosition in winPositions) {
            if (!gridSystem.GetBlockAtPosition(winPosition)) {
                return false;
            }
        }

        return true;
    }
}
