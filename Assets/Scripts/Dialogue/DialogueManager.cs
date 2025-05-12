using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
using System.Diagnostics;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Story currentStory;
    public bool isDialogueActive;

    public InkDialogueController dialogueController;

    // Map that links keys to dialogue file paths in Resources
    private Dictionary<string, string> dialogueMap = new Dictionary<string, string>
    {
        { "TestDialogue", "Dialogue Files/Test Dialogue/TestDialogue" }
    };

    private void Awake()
    {
        // Set up singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Starts a dialogue by key, with optional knot and mode
    public void StartDialogue(string dialogueKey, string knotName = null, InkDialogueController.DialogueMode mode = InkDialogueController.DialogueMode.Regular)
    {
        if (!isDialogueActive)
        {
            if (dialogueMap.ContainsKey(dialogueKey))
            {
                TextAsset inkJSON = Resources.Load<TextAsset>(dialogueMap[dialogueKey]);

                if (inkJSON != null)
                {
                    // Create new Ink story
                    currentStory = new Story(inkJSON.text);

                    // Hook up external functions
                    TimelineDialogueManager.Instance.InitializeInkFunctions(currentStory);

                    isDialogueActive = true;

                    // Jump to knot if specified
                    if (!string.IsNullOrEmpty(knotName) && currentStory.KnotContainerWithName(knotName) != null)
                    {
                        currentStory.ChoosePathString(knotName);
                    }

                    // Start dialogue
                    dialogueController.InitiateDialogue(currentStory, mode);
                }
                else
                {
                    Debug.LogError($"Dialogue file not found for key: {dialogueKey}");
                }
            }
            else
            {
                Debug.LogError($"Dialogue key not found: {dialogueKey}");
            }
        }
    }

    // Starts regular (normal) dialogue
    public void StartRegularDialogue(string dialogueKey)
    {
        StartDialogue("TestDialogue", dialogueKey, InkDialogueController.DialogueMode.Regular);
    }

    // Starts comment-style dialogue
    public void StartCommentDialogue(string dialogueKey)
    {
        StartDialogue("TestDialogue", dialogueKey, InkDialogueController.DialogueMode.Comments);
    }

    // Starts chat-style dialogue
    public void StartChatDialogue(string dialogueKey)
    {
        StartDialogue("TestDialogue", dialogueKey, InkDialogueController.DialogueMode.Chat);
    }

    // Ends the current dialogue
    public void EndDialogue()
    {
        isDialogueActive = false;
        DialogueUIController.Instance.DisableDialogueUI();
    }

    // Returns whether a dialogue is currently active
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
