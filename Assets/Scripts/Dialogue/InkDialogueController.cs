using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;

public class InkDialogueController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;

    [SerializeField]
    private GameObject optionsPanel = null;

    [SerializeField]
    private TMP_Text textPrefab;
    [SerializeField]
    private Button buttonPrefab;

    [SerializeField]
    private GameObject textPanel = null;

    [SerializeField]
    private TMP_Text speakerText; // Add a UI element for the speaker's name

    private bool isWaitingForClick = false;
    private string currentSpeaker = null; // Store the current speaker

    //void Start()
    //{
    //    ClearOptions();
    //    ClearTextPanel();
    //    StartStory();
    //}

    void Update()
    {
        if (isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            isWaitingForClick = false;
            RefreshView();
        }
    }

    public void InitiateStorySequence()
    {
        ClearOptions();
        ClearTextPanel();
        StartStory();
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        DialogueUIController.Instance.EnableDialogueUI();
        RefreshView();
    }

    void RefreshView()
    {
        if (isWaitingForClick) return;

        ClearOptions();

        if (story.canContinue)
        {
            ClearTextPanel();

            string text = story.Continue().Trim();
            string speaker = GetSpeaker(); // Get the speaker from tags

            if (speaker == "None")
            {
                // Explicitly clear the speaker when "None" is specified
                speakerText.text = " ";
                currentSpeaker = " ";
            }
            else if (!string.IsNullOrEmpty(speaker))
            {
                currentSpeaker = speaker; // Update the current speaker
                speakerText.text = currentSpeaker; // Display the speaker's name
            }
            else if (currentSpeaker != null)
            {
                // If the current speaker exists, retain it for continuity
                speakerText.text = currentSpeaker;
            }
            else
            {
                // No speaker context (system talking)
                speakerText.text = ""; // Clear the speaker text
                currentSpeaker = null; // Reset current speaker
            }

            CreateContentView(text);
            isWaitingForClick = true;
        }
        else if (story.currentChoices.Count > 0)
        {
            DialogueUIController.Instance.EnableDialogueOptions();
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        }
        else
        {
            DialogueUIController.Instance.EnableDialogueOptions();
            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate
            {
                DialogueUIController.Instance.DisableDialogueOptions();
                StartStory();
            });
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
        story.ChooseChoiceIndex(choice.index);
        DialogueUIController.Instance.DisableDialogueOptions();
        RefreshView();
    }

    void CreateContentView(string text)
    {
        TMP_Text storyText = Instantiate(textPrefab) as TMP_Text;
        storyText.text = text;
        storyText.transform.SetParent(textPanel.transform, false);
    }

    Button CreateChoiceView(string text)
    {
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(optionsPanel.transform, false);

        TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text>();
        choiceText.text = text;

        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;

        return choice;
    }

    void ClearOptions()
    {
        int childCount = optionsPanel.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            Destroy(optionsPanel.transform.GetChild(i).gameObject);
        }
    }

    void ClearTextPanel()
    {
        int childCount = textPanel.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            Destroy(textPanel.transform.GetChild(i).gameObject);
        }
    }
}
