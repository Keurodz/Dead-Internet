using UnityEngine;

public class StartLevelButton : MonoBehaviour
{
    public void OnClick()
    {
        SokobanDungeonManager.Instance.EnterDungeon();
    }
}
