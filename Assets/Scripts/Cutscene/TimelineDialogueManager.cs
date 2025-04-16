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

    // Reference to AudioManager (optional if using singleton pattern)
    [SerializeField] private AudioManager audioManager;

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

        // Find AudioManager if not assigned
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
            if (audioManager == null)
            {
                Debug.LogWarning("AudioManager not found in scene. Music functionality will be disabled.");
            }
        }
    }

    public void InitializeInkFunctions(Story story)
    {
        // Register the timeline function with the current story instance
        story.BindExternalFunction("PLAY_CUTSCENE", (string timelineId) =>
        {
            StartCoroutine(PlayTimelineAndWait(timelineId));
        });

        // Register music functions with the story
        story.BindExternalFunction("PLAY_MUSIC", (string trackName) =>
        {
            PlayMusic(trackName);
        });

        story.BindExternalFunction("STOP_MUSIC", () =>
        {
            StopMusic();
        });

        // Add a function to play music with a specific cutscene
        story.BindExternalFunction("PLAY_CUTSCENE_WITH_MUSIC", (string timelineId, string trackName) =>
        {
            StartCoroutine(PlayTimelineWithMusic(timelineId, trackName));
        });
    }

    public void PlayTriggerCutscene(string cutsceneId)
    {
        StartCoroutine(PlayTimelineAndWait(cutsceneId));
    }

    // Play music using AudioManager
    public void PlayMusic(string trackName)
    {
        if (audioManager != null)
        {
            audioManager.PlayMusic(trackName);
        }
        else
        {
            Debug.LogWarning($"Cannot play music track '{trackName}'. AudioManager not found.");
        }
    }

    // Stop music using AudioManager
    public void StopMusic()
    {
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }
    }

    // Play cutscene with specific music
    private IEnumerator PlayTimelineWithMusic(string cutsceneId, string trackName)
    {
        // Play the music first
        PlayMusic(trackName);

        // Then play the cutscene
        yield return StartCoroutine(PlayTimelineAndWait(cutsceneId));
    }

    // Play Cutscene and wait until its done playing 
    private IEnumerator PlayTimelineAndWait(string cutsceneId)
    {
        bool isInDialogue = DialogueManager.Instance.isDialogueActive;
        if (!cutsceneMap.TryGetValue(cutsceneId, out PlayableAsset cutscene))
        {
            Debug.LogError($"Cutscene with ID {cutsceneId} not found!");
            yield break;
        }
        DialogueManager.Instance.dialogueController.inCutscene = true;
        isTimelinePlaying = true;
        if (isInDialogue) { PauseDialogue(); }
        PauseDialogue();
        director.playableAsset = cutscene;
        BindTimelineTracks(cutsceneId);
        director.Play();
        // Wait until timeline finishes
        while (isTimelinePlaying)
        {
            yield return null; // Wait until OnTimelineCompleted is called
        }
        DialogueManager.Instance.dialogueController.inCutscene = false;
        // Resume dialogue
        if (isInDialogue)
        {
            DialogueUIController.Instance.EnableDialogueUI();
            DialogueManager.Instance.dialogueController.RefreshView();
            DialogueUIController.Instance.EnablePortrait();
        }
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