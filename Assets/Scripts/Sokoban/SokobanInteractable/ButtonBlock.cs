using UnityEngine;

public class ButtonBlock : MonoBehaviour, ISokobanInteractable
{
    private Vector2Int position;
    private SokobanGridSystem gridSystem;

    public InteractableObjectType Type() {
        return InteractableObjectType.ButtonBlockObject;
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
        this.position = position;
        gridSystem = FindAnyObjectByType<SokobanGridSystem>();
        transform.position = gridSystem.grid.GetCellCenterWorld((Vector3Int)position);
    }
}
