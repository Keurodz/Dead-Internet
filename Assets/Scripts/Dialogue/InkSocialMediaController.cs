using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;

public class InkSocialMediaController : MonoBehaviour
{
    // Event for other systems to subscribe when a new story starts
    public static event Action<Story> OnCreateStory;

    [SerializeField] private TextAsset inkJSONAsset = null;
    public Story story;

    [Header("Post UI Elements")]
    [SerializeField] private TMP_Text postTitleText;
    [SerializeField] private TMP_Text postContentText;
    [SerializeField] private GameObject postImagePanel;

    [Header("Comment UI Elements")]
    [SerializeField] private GameObject commentPanel;      // Container for comments
    [SerializeField] private TMP_Text commentPrefab;       // Comment text template
    [SerializeField] private Button buttonPrefab;          // Choice button template

    private string currentSpeaker = null;
    private bool isWaitingForReply = false;
    private string commentHistory = "";  // Stores the full thread of comments for context

    // Start the story when the scene loads
    void Start()
    {
        StartStory();
    }

    // Wait for mouse click to continue after a line of dialogue
    void Update()
    {
        if (isWaitingForReply && Input.GetMouseButtonDown(0))
        {
            isWaitingForReply = false;
            RefreshView();
        }
    }

    // Create a new Ink story instance and begin displaying content
    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        OnCreateStory?.Invoke(story);
        RefreshView();
    }

    // Main logic to show the next part of the story or display choices
    void RefreshView()
    {
        if (isWaitingForReply) return;

        ClearComments();

        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            string speaker = GetSpeaker();

            if (!string.IsNullOrEmpty(speaker))
                currentSpeaker = speaker;

            if (string.IsNullOrEmpty(commentHistory))
            {
                // First line appears as the main post
                UpdatePostContent(text);
            }
            else
            {
                // Later lines appear as comment replies
                AppendComment(currentSpeaker, text);
            }

            isWaitingForReply = true;
        }
        else if (story.currentChoices.Count > 0)
        {
            // Display player response options
            foreach (Choice choice in story.currentChoices)
            {
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate { OnClickChoiceButton(choice); });
            }
        }
    }

    // Pulls the speaker name from the current story tags
    string GetSpeaker()
    {
        foreach (string tag in story.currentTags)
        {
            if (tag.StartsWith("Character:"))
                return tag.Substring("Character:".Length).Trim();
        }
        return null;
    }

    // Handles player choosing a dialogue option
    void OnClickChoiceButton(Choice choice)
    {
        // Add player choice as a comment
        AppendComment("You", choice.text);
        commentHistory += "You: " + choice.text + "\n";

        // Continue the story based on choice
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    // Updates the main post (first message) content
    void UpdatePostContent(string content)
    {
        postContentText.text = content;
        postImagePanel.SetActive(false); // TODO: Update if we have linked images to posts
    }

    // Adds a comment (NPC or player) to the comment thread
    void AppendComment(string speaker, string content)
    {
        TMP_Text newComment = Instantiate(commentPrefab, commentPanel.transform);
        newComment.text = $"<b>{speaker}:</b> {content}";
        commentHistory += $"{speaker}: {content}\n";
    }

    // Creates a choice button for the player to click
    Button CreateChoiceView(string text)
    {
        Button choice = Instantiate(buttonPrefab, commentPanel.transform);
        TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text>();
        choiceText.text = text;
        return choice;
    }

    // Clears out old comments and choices
    void ClearComments()
    {
        foreach (Transform child in commentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
