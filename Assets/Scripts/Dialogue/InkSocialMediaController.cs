using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;

public class InkSocialMediaController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;

    [Header("Post UI Elements")]
    [SerializeField] private TMP_Text postTitleText;
    [SerializeField] private TMP_Text postContentText;
    [SerializeField] private GameObject postImagePanel;

    [Header("Comment UI Elements")]
    [SerializeField] private GameObject commentPanel; // Comment container
    [SerializeField] private TMP_Text commentPrefab; // Comment template
    [SerializeField] private Button buttonPrefab; // Choice template

    private string currentSpeaker = null;
    private bool isWaitingForReply = false;
    private string commentHistory = ""; // Keeps track of previous comments for context

    void Start()
    {
        StartStory();
    }

    void Update()
    {
        if (isWaitingForReply && Input.GetMouseButtonDown(0))
        {
            isWaitingForReply = false;
            RefreshView();
        }
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        OnCreateStory?.Invoke(story);
        RefreshView();
    }

    void RefreshView()
    {
        if (isWaitingForReply) return;

        ClearComments();

        if (story.canContinue)
        {
            string text = story.Continue().Trim();
            string speaker = GetSpeaker();

            if (!string.IsNullOrEmpty(speaker))
            {
                currentSpeaker = speaker;
            }

            if (string.IsNullOrEmpty(commentHistory))
            {
                // First message appears in the post section
                UpdatePostContent(text);
            }
            else
            {
                // Subsequent messages appear in the comments section
                AppendComment(currentSpeaker, text);
            }

            isWaitingForReply = true;
        }
        else if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate { OnClickChoiceButton(choice); });
            }
        }
    }

    string GetSpeaker()
    {
        foreach (string tag in story.currentTags)
        {
            if (tag.StartsWith("Character:"))
            {
                return tag.Substring("Character:".Length).Trim();
            }
        }
        return null;
    }

    void OnClickChoiceButton(Choice choice)
    {
        // Add player's choice to the comment history
        AppendComment("You", choice.text);
        commentHistory += "You: " + choice.text + "\n";

        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    void UpdatePostContent(string content)
    {
        postContentText.text = content;
        postImagePanel.SetActive(false); // Modify if you have images linked
    }

    void AppendComment(string speaker, string content)
    {
        TMP_Text newComment = Instantiate(commentPrefab, commentPanel.transform);
        newComment.text = $"<b>{speaker}:</b> {content}";
        commentHistory += $"{speaker}: {content}\n";
    }

    Button CreateChoiceView(string text)
    {
        Button choice = Instantiate(buttonPrefab, commentPanel.transform);
        TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text>();
        choiceText.text = text;
        return choice;
    }

    void ClearComments()
    {
        foreach (Transform child in commentPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
