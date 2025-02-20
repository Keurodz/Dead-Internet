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
    public void StartDialogue(string dialogueKey, InkDialogueController.DialogueMode mode = InkDialogueController.DialogueMode.Regular)
    {
        if (dialogueMap.ContainsKey(dialogueKey))
        {
            TextAsset inkJSON = Resources.Load<TextAsset>(dialogueMap[dialogueKey]);
            if (inkJSON != null)
            {
                currentStory = new Story(inkJSON.text);
                isDialogueActive = true;
                dialogueController.InitiateDialogue(currentStory, mode); // Pass the mode to the controller
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

    // Method specifically for starting comment-style dialogue
    public void StartCommentDialogue(string dialogueKey)
    {
        StartDialogue(dialogueKey, InkDialogueController.DialogueMode.Comments);
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