using UnityEngine;

// ILevelController is an interface for classes that control the level.
public interface ILevelController
{
    // triggers the next level 
    public void NextLevel();


    // triggers the win sequence 
    public void PlayWinSequence();
    
    // triggers the restart of the level
    public void RestartLevel();
}
