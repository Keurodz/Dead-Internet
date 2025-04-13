using UnityEngine;

public class PitBlock : MonoBehaviour, ISokobanInteractable
{
    private Vector2Int position;
    private SokobanGridSystem gridSystem;

    public InteractableObjectType Type() {
        return InteractableObjectType.PitBlockObject;
    }

    public bool TryPush(Direction direction) {
        return false;
    }

    public bool TryFloat() {
        return false;
    }

    public bool IsPushable() {
        return false;
    }

    public void Initialize(Vector2Int position) {
        // Debug.Log("MovableBlock initialized at " + position);
        this.position = position;
        gridSystem = FindAnyObjectByType<SokobanGridSystem>();
        transform.position = gridSystem.grid.GetCellCenterWorld((Vector3Int)position);
    }
}
