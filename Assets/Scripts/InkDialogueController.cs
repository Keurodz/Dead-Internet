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

    private bool isWaitingForClick = false; // Tracks if waiting for player input
    private string currentLine; // Stores the current line of text

    void Start()
    {
        ClearOptions();
        ClearTextPanel();
        StartStory();
    }

    void Update()
    {
        if (isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            isWaitingForClick = false; // Reset waiting state
            RefreshView(); // Continue the story
        }
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story);
        RefreshView();
    }

    void RefreshView()
    {
        if (isWaitingForClick) return; // Do nothing if waiting for input

        // Clear options but not text panel
        ClearOptions();

        // Show the next line if available
        if (story.canContinue)
        {
            // Clear the text panel when advancing the story
            ClearTextPanel();

            currentLine = story.Continue().Trim(); // Get the next line
            CreateContentView(currentLine); // Display the line
            isWaitingForClick = true; // Wait for player input
        }
        // Display all the choices, if there are any!
        else if (story.currentChoices.Count > 0)
        {
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
            // If no text or choices remain, show "End of story" option
            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate
            {
                StartStory();
            });
        }
    }

    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
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
