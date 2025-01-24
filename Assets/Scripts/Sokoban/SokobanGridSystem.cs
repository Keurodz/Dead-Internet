using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class SokobanGridSystem : MonoBehaviour
{
    [SerializeField] public Grid grid; // reference to the world grid
    [SerializeField] public LayerMask blockingLayer; // reference to the blocking layer (pushable boxes)
    private Dictionary<Vector2Int, GameObject> gridDictionary = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<GameObject, bool> movingObjects = new Dictionary<GameObject, bool>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateGridWithBlocks();
    }

    // scans the scene for blocks and populates the grid array with them
    private void PopulateGridWithBlocks()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Sokoban");

        foreach (GameObject block in blocks)
        {
            Vector2Int gridPosition = (Vector2Int)grid.WorldToCell(block.transform.position);
            if (!gridDictionary.ContainsKey(gridPosition))
            {
                gridDictionary[gridPosition] = block;
                block.GetComponent<ISokobanInteractable>().Initialize(gridPosition);
            }
        }
    }

    // attemps to move the block at the given position in the given direction
    // returns true if the block was moved, false otherwise
    public bool TryToPushBox(Vector2Int position, Direction direction) {
        Vector2Int targetPosition = position + GetDirectionVector(direction);

        if (!gridDictionary.ContainsKey(targetPosition))
        {
            if (gridDictionary.TryGetValue(position, out GameObject box))
            {
                if (movingObjects.ContainsKey(box) && movingObjects[box])
                {
                    return false;
                }

                movingObjects[box] = true;

                gridDictionary.Remove(position);
                gridDictionary[targetPosition] = box;

                Vector3 targetWorldPosition = grid.GetCellCenterWorld((Vector3Int)targetPosition);
                StartCoroutine(TweenToPosition(box, targetWorldPosition, 0.3f)); 
                return true;
            }
        }
        return false;
    }

    // coroutine to tween the position
    private IEnumerator TweenToPosition(GameObject box, Vector3 targetPosition, float duration) {
        Vector3 startPosition = box.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            box.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        box.transform.position = targetPosition;

        if (movingObjects.ContainsKey(box))
        {
            movingObjects[box] = false;
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
}
