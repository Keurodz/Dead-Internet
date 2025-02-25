using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Scriptable Objects/Cutscene")]
public class CutsceneAsset : ScriptableObject
{
    public string cutsceneId;
    public PlayableAsset timelineAsset;
}
