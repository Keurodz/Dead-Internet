using System;
using UnityEngine;

// This class stores the information for an interactable object in the sokoban game.
[System.Serializable]
public class InteractableObject
{
    // the position of the interactable object in the grid system
    public Vector2Int gridPosition;

    // the type of the interactable object
    public InteractableObjectType type;
}

// This enum stores the possible types of the interactable object.
public enum InteractableObjectType
{
    MovableBlockObject, 
    ImmovableBlockObject, 
    ButtonBlockObject
}
