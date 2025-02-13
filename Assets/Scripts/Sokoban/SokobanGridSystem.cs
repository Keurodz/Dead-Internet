using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class SokobanGridSystem : MonoBehaviour
{
    [SerializeField] public Grid grid; // reference to the world grid
    [SerializeField] public LayerMask blockingLayer; // reference to the blocking layer (pushable boxes)

    public GameObject movablePrefab;
    public GameObject immovablePrefab;
    public GameObject buttonPrefab;

    private Dictionary<Vector2Int, GameObject> gridDictionary = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<GameObject, bool> movingObjects = new Dictionary<GameObject, bool>();
    private Vector2Int gridBounds;

    // the positions where buttons are located
    private List<Vector2Int> winPositions = new List<Vector2Int>();

    // populate the grid with the blocks of the level data.
    // returns false if there is a block at an occupied position, meaning the level data is invalid
    public bool PopulateGridWithBlocks(SokobanLevelData levelData)
    {
        gridBounds = levelData.bounds;
        foreach (InteractableObject element in levelData.interactableObjects)
        {
            Vector2Int gridPosition = element.gridPosition;

            // spawns the interactable object in the world
            Vector3 worldPosition = this.GetWorldPosition(gridPosition);

            GameObject prefab = (element.type == InteractableObjectType.MovableBlockObject) ? movablePrefab : 
            (element.type == InteractableObjectType.ImmovableBlockObject) ? immovablePrefab : 
            (element.type == InteractableObjectType.ButtonBlockObject) ? buttonPrefab : null;

            if (element.type == InteractableObjectType.ButtonBlockObject) {
                winPositions.Add(gridPosition);
            }
        
            GameObject interactableGameObject = Instantiate(prefab, worldPosition, Quaternion.identity);
            interactableGameObject.GetComponent<ISokobanInteractable>().Initialize(gridPosition);

            // adds the game object to the grid dictionary
            if (!gridDictionary.ContainsKey(gridPosition))
            {
                if (!(element.type == InteractableObjectType.ButtonBlockObject)) {
                    // button block do not occupy a grid position
                    gridDictionary[gridPosition] = interactableGameObject;
                }
            } else {
                return false;
            }
        }
        return true;
    }

    // attemps to move the block at the given position in the given direction
    // returns true if the block was moved, false otherwise
    public bool TryToPushBox(Vector2Int position, Direction direction) {
        Vector2Int targetPosition = position + GetDirectionVector(direction);
        // if there are no blocks at the target position and there is a block at the current position
        if (CanPushBox(position, targetPosition))
        {
            GameObject box = gridDictionary[position];
            // if the block is already moving, return false
            if (movingObjects.ContainsKey(box) && movingObjects[box] || 
                !box.GetComponent<ISokobanInteractable>().IsPushable()) {
                Debug.Log("cannot push");
                Debug.Log("box: " + box);
                return false;
            } else {
                Debug.Log("pushing");
                Debug.Log("box: " + box);

                movingObjects[box] = true;

                StartCoroutine(TweenToPosition(box, position, targetPosition, 1.5f)); 
                return true;
            }
        }
        return false;
    }

    // can the box at the given position be pushed to the new position?
    private bool CanPushBox(Vector2Int originalPosition,Vector2Int targetPosition) {
        return !gridDictionary.ContainsKey(targetPosition) && 
            gridDictionary.ContainsKey(originalPosition) &&
            targetPosition.x >= 1 && targetPosition.x <= gridBounds.x &&
            targetPosition.y >= 1 && targetPosition.y <= gridBounds.y;
    }

    // coroutine to tween the position
    private IEnumerator TweenToPosition(GameObject box, Vector2Int gridStartPosition, Vector2Int gridTargetPosition, float duration) {
        Vector3 worldStartPosition = box.transform.position;
        Vector3 worldTargetPosition = grid.GetCellCenterWorld((Vector3Int)gridTargetPosition);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            box.transform.position = Vector3.Lerp(worldStartPosition, worldTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        box.transform.position = worldTargetPosition;

        if (movingObjects.ContainsKey(box))
        {
            movingObjects[box] = false;

            gridDictionary.Remove(gridStartPosition);
            gridDictionary[gridTargetPosition] = box;
        }
    }

    // get the vector corresponding to the given direction
    public Vector2Int GetDirectionVector(Direction direction) {
        switch (direction) {
            case Direction.Up:
                return new Vector2Int(0, 1);
            case Direction.Down:
                return new Vector2Int(0, -1);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Right:
                return new Vector2Int(1, 0);
            default:
                return new Vector2Int(0, 0);
        }
    }

    // gets the world position of the given grid position 
    public Vector3 GetWorldPosition(Vector2Int gridPosition) {
        return grid.GetCellCenterWorld((Vector3Int)gridPosition);
    }

    // gets the block at the given grid position
    // returns null if there is no block at the given position
    public GameObject GetBlockAtPosition(Vector2Int gridPosition) {
        if (gridDictionary.TryGetValue(gridPosition, out GameObject block)) {
            return block;
        } else {
            return null;
        }
    }

    // checks if the win condition is met
    public bool CheckWinCondition() {
        foreach (Vector2Int position in winPositions) {
            if (!gridDictionary.ContainsKey(position)) {
                return false;
            }
        }
        return true;
    }
}
