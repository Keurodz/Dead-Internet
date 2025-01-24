using UnityEngine;

// a sokoban interactable is an interface for objects that can be interacted with 
// in the sokoban game
public interface ISokobanInteractable
{
    // try to push the interactable in the given direction
    bool TryPush(Direction direction);

    // is the interactable pushable?
    bool IsPushable();

    // called to initialize the interactable at the given position
    void Initialize(Vector2Int position);
}

public enum Direction {
    Up,
    Down,
    Left,
    Right
}