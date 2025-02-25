using UnityEngine;

[CreateAssetMenu(fileName = "SokobanLevelData", menuName = "Scriptable Objects/SokobanLevelData")]
public class SokobanLevelData : ScriptableObject
{
    // the name of the level 
    public string levelName;

    // the dimensions of the level
    public Vector2Int bounds; 

    // the lower left corner of the level in world space
    public Vector2Int anchorPoint;

    // the items in the level 
    [SerializeField]
    public InteractableObject[] interactableObjects;
}


