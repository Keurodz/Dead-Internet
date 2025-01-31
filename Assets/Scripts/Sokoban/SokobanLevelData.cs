using UnityEngine;

[CreateAssetMenu(fileName = "SokobanLevelData", menuName = "Scriptable Objects/SokobanLevelData")]
public class SokobanLevelData : ScriptableObject
{
    // the dimensions of the level
    public Vector2 bounds; 

    // the lower left corner of the level in world space
    public Vector2 anchorPoint;

    // the items in the level 
    [SerializeField]
    public InteractableObject[] interactableObjects;
}


