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

    // the amount of alien ability ammo in the level
    public int alienAbilityAmmo = 0;

    // the items in the level 
    public InteractableObject[] interactableObjects;

    // gets the environment name for the level
    // the name of the environment is the dimensions of the level
    // ex. 6x6E
    public string GetEnvironmentName() {
        return bounds.x + "x" + bounds.y + "E";
    }
}


