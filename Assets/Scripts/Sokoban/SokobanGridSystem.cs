using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class SokobanGridSystem : MonoBehaviour
{
    [SerializeField] public Grid grid; // reference to the world grid
    [SerializeField] public LayerMask blockingLayer; // reference to the blocking layer (pushable boxes)
    private List<List<GameObject>> gridArray = new List<List<GameObject>>(); // 2D array of GameObjects


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // attemps to move the block at the given position in the given direction
    // returns true if the block was moved, false otherwise
    public bool TryToPushBox(Vector2Int position, Direction direction) {
        Vector2Int targetPosition = position + GetDirectionVector(direction);
        if (IsBlockAtPosition(targetPosition)) {
            return false;
        }
        gridArray[targetPosition.x][targetPosition.y] = gridArray[position.x][position.y];
        gridArray[position.x][position.y] = null;
        return true;
    }

    // is there a block occupying the given position?
    public bool IsBlockAtPosition(Vector2Int position) {
        return gridArray[(int)position.x][(int)position.y] != null;
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

public enum Direction {
    Up,
    Down,
    Left,
    Right
}