using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Story currentStory;
    private bool isDialogueActive;

    [SerializeField]
    private InkDialogueController dialogueController;

    // Mapping character/event keys to Ink file paths
    private Dictionary<string, string> dialogueMap = new Dictionary<string, string>
    {
        { "TestDialogue", "Dialogue Files/Test Dialogue/TestDialogue" },
        // Add more mappings as needed
    };


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Load and start a dialogue by key
    public void StartDialogue(string dialogueKey, string knotName = null, InkDialogueController.DialogueMode mode = InkDialogueController.DialogueMode.Regular)
    {
        if (dialogueMap.ContainsKey(dialogueKey))
        {
            TextAsset inkJSON = Resources.Load<TextAsset>(dialogueMap[dialogueKey]);
            if (inkJSON != null)
            {
                currentStory = new Story(inkJSON.text);
                isDialogueActive = true;

                // Jump to specific knot if provided
                if (!string.IsNullOrEmpty(knotName) && currentStory.KnotContainerWithName(knotName) != null)
                {
                    currentStory.ChoosePathString(knotName);
                }

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


    public void StartRegularDialogue(string dialogueKey)
    {
        StartDialogue("TestDialogue", dialogueKey, InkDialogueController.DialogueMode.Regular);
    }

    // Method specifically for starting comment-style dialogue
    public void StartCommentDialogue(string dialogueKey)
    {
        StartDialogue("TestDialogue", dialogueKey, InkDialogueController.DialogueMode.Comments);
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        DialogueUIController.Instance.DisableDialogueUI();
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}