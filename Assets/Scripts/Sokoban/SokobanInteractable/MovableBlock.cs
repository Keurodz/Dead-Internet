using UnityEngine;

public class MovableBlock : MonoBehaviour, ISokobanInteractable
{
    private Vector2Int position;
    private SokobanGridSystem gridSystem;

    public InteractableObjectType Type() {
        return InteractableObjectType.MovableBlockObject;
    }

    public bool TryPush(Direction direction) {
        if (gridSystem.TryToPushBlock(position, direction)) {
            position += gridSystem.GetDirectionVector(direction);
            return true;
        } else {
            return false;
        }
    }

    public bool TryFloat() {
        return false;
    }

    public bool IsPushable() {
        return true;
    }

    public void Initialize(Vector2Int position) {
        // Debug.Log("MovableBlock initialized at " + position);
        this.position = position;
        gridSystem = FindAnyObjectByType<SokobanGridSystem>();
        transform.position = gridSystem.grid.GetCellCenterWorld((Vector3Int)position);
    }
}
