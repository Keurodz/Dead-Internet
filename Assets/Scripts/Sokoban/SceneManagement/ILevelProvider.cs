using UnityEngine;
using System;

// ILevelProvider is an interface for classes that provide level data.
public interface ILevelProvider
{
    // returns the current dungeon level index
    int CurrentDungeonLevelIndex();
    // returns the total number of dungeon levels
    int TotalDungeonLevels();

    // triggers when the level changes 
    event Action OnLevelChanged;
}
