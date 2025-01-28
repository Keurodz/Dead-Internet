using UnityEngine;

public class SokobanPuzzleSystem : MonoBehaviour
{
    public SokobanLevelData levelData;
    public SokobanGridSystem gridSystem;

    public GameObject movablePrefab;
    public GameObject immovablePrefab;
    public GameObject buttonPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridSystem = GetComponent<SokobanGridSystem>();
        LoadLevelData();
        gridSystem.PopulateGridWithBlocks();
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
            GameObject interactableGameObject = Instantiate(prefab, worldPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
