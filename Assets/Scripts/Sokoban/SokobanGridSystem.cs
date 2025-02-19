using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class SokobanGridSystem : MonoBehaviour
{
    [SerializeField] public Grid grid; // reference to the world grid
    [SerializeField] public LayerMask blockingLayer; // reference to the blocking layer (pushable boxes)

    // prefab for the movable block
    public GameObject movablePrefab;
    // prefab for the immovable block
    public GameObject immovablePrefab;
    // prefab for the button block
    public GameObject buttonPrefab;

    // dictionary to store the grid positions and the game objects at those positions
    private Dictionary<Vector2Int, GameObject> gridDictionary = new Dictionary<Vector2Int, GameObject>();
    // dictionary to store the grid positions of floating objects
    private Dictionary<Vector2Int, GameObject> floatingObjects = new Dictionary<Vector2Int, GameObject>();
    // dictionary to store the moving objects and whether they are moving
    private Dictionary<GameObject, bool> movingObjects = new Dictionary<GameObject, bool>();
    // the bounds of the grid
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
    public bool TryToPushBlock(Vector2Int position, Direction direction) {
        Vector2Int targetPosition = position + GetDirectionVector(direction);
        // if there are no blocks at the target position and there is a block at the current position
        if (CanPushBlock(position, targetPosition))
        {
            GameObject block = gridDictionary[position];
            // if the block is already moving, return false
            if (movingObjects.ContainsKey(block) && movingObjects[block] || 
                !block.GetComponent<ISokobanInteractable>().IsPushable()) {
                return false;
            } else {
                StartCoroutine(TweenToPosition(block, position, targetPosition)); 
                return true;
            }
        }
        return false;
    }

    // attemps to float an object at the given position 
    // returns true if the object was floated, false otherwise
    public bool TryToFloatBlock(Vector2Int position) {
        // if there is a block at the given position and it is not already floating
        if (gridDictionary.ContainsKey(position) && !floatingObjects.ContainsKey(position)) {
            GameObject block = gridDictionary[position];
            if (movingObjects.ContainsKey(block) && movingObjects[block]) {
                return false;
            }
            StartCoroutine(FloatBlock(block, position));
            return true;
        } else if (floatingObjects.ContainsKey(position) && !gridDictionary.ContainsKey(position)) {
            GameObject block = floatingObjects[position];
            if (movingObjects.ContainsKey(block) && movingObjects[block]) {
                return false;
            }
            StartCoroutine(UnfloatBlock(block, position));
            return true;
        }
        return false;
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

    // can the box at the given position be pushed to the new position?
    private bool CanPushBlock(Vector2Int originalPosition,Vector2Int targetPosition) {
        return !gridDictionary.ContainsKey(targetPosition) && 
            gridDictionary.ContainsKey(originalPosition) &&
            targetPosition.x >= 1 && targetPosition.x <= gridBounds.x &&
            targetPosition.y >= 1 && targetPosition.y <= gridBounds.y;
    }

    // coroutine to tween the given block from the start position to the target position
    private IEnumerator TweenToPosition(GameObject block, Vector2Int gridStartPosition, Vector2Int gridTargetPosition) {
        movingObjects[block] = true;

        Vector3 worldStartPosition = block.transform.position;
        Vector3 worldTargetPosition = grid.GetCellCenterWorld((Vector3Int)gridTargetPosition);

        float elapsedTime = 0f;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            block.transform.position = Vector3.Lerp(worldStartPosition, worldTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = worldTargetPosition;

        if (movingObjects.ContainsKey(block))
        {
            movingObjects[block] = false;

            gridDictionary.Remove(gridStartPosition);
            gridDictionary[gridTargetPosition] = block;
        }
    }
    
    // coroutine to float the given block 
    private IEnumerator FloatBlock(GameObject block, Vector2Int gridPosition) {
        movingObjects[block] = true;
        float elapsedTime = 0f;
        float floatDuration = 1.5f;
        float floatHeight = 3f;
        Vector3 worldPosition = block.transform.position;
        Vector3 destinationPosition = worldPosition + (Vector3.up * floatHeight);

        while (elapsedTime < floatDuration)
        {
            block.transform.position = Vector3.Lerp(worldPosition, destinationPosition, elapsedTime / floatDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = destinationPosition;
        gridDictionary.Remove(gridPosition);
        floatingObjects[gridPosition] = block;
        movingObjects[block] = false;
    }

    private IEnumerator UnfloatBlock(GameObject block, Vector2Int gridPosition) {
        movingObjects[block] = true;
        float elapsedTime = 0f;
        float floatDuration = 1.5f;
        float floatHeight = 3f;
        Vector3 worldPosition = block.transform.position;
        Vector3 destinationPosition = worldPosition + (Vector3.down * floatHeight);

        while (elapsedTime < floatDuration)
        {
            block.transform.position = Vector3.Lerp(worldPosition, destinationPosition, elapsedTime / floatDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = destinationPosition;
        floatingObjects.Remove(gridPosition);
        gridDictionary[gridPosition] = block;
        movingObjects[block] = false;
    }
}
