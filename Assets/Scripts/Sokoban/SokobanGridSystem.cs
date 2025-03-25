using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

/*
This class handles the sokoban grid system, standardizing the sokoban rules
and enforcing all the interactions with the sokoban blocks 
for both grid and world position representation.
*/
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
    
    // list of all the GameObject references 
    private List<GameObject> interactableObjects = new List<GameObject>();

    // dictionary to store the grid positions and the game objects at those positions
    private Dictionary<Vector2Int, GameObject> gridDictionary = new Dictionary<Vector2Int, GameObject>();
    // dictionary to store the grid positions of floating objects
    private Dictionary<Vector2Int, GameObject> floatingObjects = new Dictionary<Vector2Int, GameObject>();
    // dictionary to store the moving objects and whether they are moving
    private Dictionary<GameObject, bool> movingObjects = new Dictionary<GameObject, bool>();
    // the bounds of the grid
    private Vector2Int gridBounds;
    // the offset of the grid
    private Vector2Int gridOffset;

    // the positions where buttons are located
    private List<Vector2Int> winPositions = new List<Vector2Int>();

    // populate the grid with the blocks of the level data.
    // returns false if there is a block at an occupied position, meaning the level data is invalid
    public bool PopulateGridWithBlocks(SokobanLevelData levelData)
    {
        ClearGrid();

        gridBounds = levelData.bounds;
        gridOffset = levelData.anchorPoint;
        foreach (InteractableObject element in levelData.interactableObjects)
        {
            Vector2Int gridPosition = element.gridPosition + new Vector2Int(levelData.anchorPoint.x, levelData.anchorPoint.y); // offset by 1 to account for the grid padding

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

            interactableObjects.Add(interactableGameObject);

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

    private void ClearGrid() {
        foreach (GameObject block in interactableObjects) {
            Destroy(block);
        }
        gridDictionary.Clear();
        floatingObjects.Clear();
        movingObjects.Clear();
        winPositions.Clear();
        interactableObjects.Clear();
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

    // gets the grid position of the given world position
    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        Vector3Int gridPosition = grid.WorldToCell(worldPosition);
        return new Vector2Int(gridPosition.x, gridPosition.y);
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

    // checks if the given position is traversable (nothing is blocking it)
    public bool IsCellTraversable(Vector2Int position) {
        return !gridDictionary.ContainsKey(position);
    }

    // can the box at the given position be pushed to the new position?
    private bool CanPushBlock(Vector2Int originalPosition,Vector2Int targetPosition) {
        return !gridDictionary.ContainsKey(targetPosition) && 
            gridDictionary.ContainsKey(originalPosition) &&
            targetPosition.x >= 1 + gridOffset.x  && targetPosition.x <= gridBounds.x + gridOffset.x &&
            targetPosition.y >= 1 + gridOffset.y && targetPosition.y <= gridBounds.y + gridOffset.y;
    }

    // coroutine to tween the given block from the start position to the target position
    private IEnumerator TweenToPosition(GameObject block, Vector2Int gridStartPosition, Vector2Int gridTargetPosition) {
        movingObjects[block] = true;

        Vector3 worldStartPosition = block.transform.position;
        Vector3 worldTargetPosition = grid.GetCellCenterWorld((Vector3Int)gridTargetPosition);

        float elapsedTime = 0f;
        float duration = 0.6f;

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
        float floatDuration = 0.7f;
        float floatHeight = 3f;
        Vector3 worldPosition = block.transform.position;
        Vector3 destinationPosition = worldPosition + (Vector3.up * floatHeight);

        // material transparency 
        MeshRenderer blockMesh = block.GetComponentInChildren<MeshRenderer>();
        Color initialColor = blockMesh.material.color;
        float startAlpha = initialColor.a;
        float targetAlpha = 0.4f;

        while (elapsedTime < floatDuration)
        {
            float timeChange = elapsedTime / floatDuration;
            block.transform.position = Vector3.Lerp(worldPosition, destinationPosition, timeChange);

            Color newColor = blockMesh.material.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, timeChange);
            blockMesh.material.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = destinationPosition;
        gridDictionary.Remove(gridPosition);
        floatingObjects[gridPosition] = block;
        movingObjects[block] = false;
    }

    // coroutine to unfloat the given block at the given position
    private IEnumerator UnfloatBlock(GameObject block, Vector2Int gridPosition) {
        movingObjects[block] = true;
        float elapsedTime = 0f;
        float floatDuration = 0.7f;        
        float floatHeight = 3f;
        Vector3 worldPosition = block.transform.position;
        Vector3 destinationPosition = worldPosition + (Vector3.down * floatHeight);
       
        // material transparency 
        MeshRenderer blockMesh = block.GetComponentInChildren<MeshRenderer>();
        Color initialColor = blockMesh.material.color;
        float startAlpha = initialColor.a;
        float targetAlpha = 1f;

        while (elapsedTime < floatDuration)
        {
            float timeChange = elapsedTime / floatDuration;

            Color newColor = blockMesh.material.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, timeChange);
            blockMesh.material.color = newColor;

            block.transform.position = Vector3.Lerp(worldPosition, destinationPosition, timeChange);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        block.transform.position = destinationPosition;
        floatingObjects.Remove(gridPosition);
        gridDictionary[gridPosition] = block;
        movingObjects[block] = false;
    }

    // runs A* algorithm to get the path from the start to the end position
    public List<Vector2Int> GetPath(Vector2Int start, Vector2Int end) {
        if (start == end) return new List<Vector2Int> { start };

        // The set of nodes to evaluate using the custom PriorityQueue
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
        openSet.Enqueue(start, 0);

        // Keeps track of the path
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Cost from start to each position
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float> {
            [start] = 0
        };

        // Estimated total cost from start to end through each position
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float> {
            [start] = Heuristic(start, end)
        };

        while (!openSet.IsEmpty()) {
            Vector2Int current = openSet.Dequeue();

            // If we reached the goal, reconstruct the path
            if (current == end) {
                return ReconstructPath(cameFrom, current);
            }

            // Evaluate neighbors (Up, Down, Left, Right)
            foreach (Vector2Int direction in new Vector2Int[] {
                new Vector2Int(0, 1),   // Up
                new Vector2Int(0, -1),  // Down
                new Vector2Int(-1, 0),  // Left
                new Vector2Int(1, 0)    // Right
            }) {
                Vector2Int neighbor = current + direction;

                // Skip if the neighbor is out of bounds or occupied
                if (!IsPositionValid(neighbor) || gridDictionary.ContainsKey(neighbor)) {
                    continue;
                }

                float tentativeGScore = gScore[current] + 1; // Assuming uniform cost of 1 per move

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, end);

                    // Check if the neighbor is not already in the priority queue
                    if (!openSet.Contains(neighbor)) {
                        openSet.Enqueue(neighbor, Mathf.RoundToInt(fScore[neighbor]));
                    }
                }
            }
        }

        // Return an empty path if no valid path found
        return new List<Vector2Int>();
    }

    // heuristics function for A*
    private float Heuristic(Vector2Int a, Vector2Int b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // reconstructs the path from the start to the goal
    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current) {
        List<Vector2Int> path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    // check if the grid position is within bounds
    private bool IsPositionValid(Vector2Int position) {
        return position.x >= 1 && position.x <= gridBounds.x &&
            position.y >= 1 && position.y <= gridBounds.y;
    }
}
