using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;

public class TimelineDialogueManager : MonoBehaviour
{
    public static TimelineDialogueManager Instance { get; private set; }

    [SerializeField] private List<CutsceneAsset> cutscenes;

    private Dictionary<string, PlayableAsset> cutsceneMap = new Dictionary<string, PlayableAsset>();
    private bool isTimelinePlaying = false;
    private string pausedDialogueState;

    private PlayableDirector director;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        director = gameObject.AddComponent<PlayableDirector>();
        director.playOnAwake = false;
        foreach (var cutscene in cutscenes)
        {
            cutsceneMap[cutscene.cutsceneId] = cutscene.timelineAsset;
        }

        // Subscribe to the stopped event
        director.stopped += OnTimelineCompleted;
    }

    public void InitializeInkFunctions(Story story)
    {
        // Register the timeline function with the current story instance
        story.BindExternalFunction("PLAY_CUTSCENE", (string timelineId) =>
        {
            StartCoroutine(PlayTimelineAndWait(timelineId));
        });
    }

    public void PlayTimelineCutscene(string cutsceneId)
    {
        if (!cutsceneMap.ContainsKey(cutsceneId))
        {
            Debug.LogError($"Cutscene with ID {cutsceneId} not found!");
            return;
        }

        // Set isTimelinePlaying to true when starting a timeline
        isTimelinePlaying = true;

        // Also, you may need to pause dialogue here
        PauseDialogue();

        director.playableAsset = cutsceneMap[cutsceneId];
        BindTimelineTracks(cutsceneId);
        director.Play();
    }

    private IEnumerator PlayTimelineAndWait(string cutsceneId)
    {
        if (!cutsceneMap.TryGetValue(cutsceneId, out PlayableAsset cutscene))
        {
            Debug.LogError($"Cutscene with ID {cutsceneId} not found!");
            yield break;
        }

        isTimelinePlaying = true;
        PauseDialogue();

        director.playableAsset = cutscene;
        BindTimelineTracks(cutsceneId);
        director.Play();

        // Wait until timeline finishes
        while (isTimelinePlaying)
        {
            yield return null; // Wait until OnTimelineCompleted is called
        }

        // Resume dialogue
        DialogueUIController.Instance.EnableDialogueUI();
        DialogueManager.Instance.dialogueController.RefreshView();
        DialogueUIController.Instance.EnablePortrait();
    }

    private void BindTimelineTracks(string cutsceneId)
    {
        var cutscene = cutsceneMap[cutsceneId];
        if (cutscene == null) return;

        foreach (var output in cutscene.outputs)
        {
            // Find the GameObject or Component in the scene that matches the track name
            var target = GameObject.Find(output.streamName);
            if (target != null)
            {
                director.SetGenericBinding(output.sourceObject, target);
            }
        }
    }

    private void OnTimelineCompleted(PlayableDirector director)
    {
        isTimelinePlaying = false; // Let coroutine know we're done
    }

    private void PauseDialogue()
    {
        if (DialogueManager.Instance.IsDialogueActive())
        {
            // Hide UI or lock interactions without ending the Ink story
            DialogueUIController.Instance.DisableDialogueUI();
        }
    }

    public bool IsTimelinePlaying()
    {
        return isTimelinePlaying;
    }

    private void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineCompleted;
        }
    }
}