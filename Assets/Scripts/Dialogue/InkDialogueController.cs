using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class InkDialogueController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    public enum DialogueMode { Regular, Comments }

    [SerializeField] private TextAsset inkJSONAsset;
    [SerializeField] private GameObject optionsPanel, textPanel, commentsPanel;
    [SerializeField] private TMP_Text textPrefab, speakerText;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private GameObject commentPrefab;

    private Story story;
    private bool isWaitingForClick = false;
    private string currentSpeaker = null;
    private DialogueMode currentMode = DialogueMode.Regular;
    private ScrollRect scrollRect;

    public bool inCutscene = false;

    void Update()
    {
        if (!inCutscene) {
            if (isWaitingForClick && Input.GetMouseButtonDown(0))
            {
                isWaitingForClick = false;
                RefreshView();
            }
        }
    }

    public void SetCommentPrefab(GameObject commentPrefab)
    {
        this.commentPrefab = commentPrefab;
    }

    public void SetScrollRect(ScrollRect scrollRect)
    {
        this.scrollRect = scrollRect;
    }

    public void SetCommentsPanel(GameObject commentsPanel)
    {
        this.commentsPanel = commentsPanel;
    }

    public void InitiateDialogue(Story story, DialogueMode mode = DialogueMode.Regular)
    {
        this.story = story;
        currentMode = mode;
        ClearUI();

        if (mode == DialogueMode.Regular)
        {
            DialogueUIController.Instance.EnableDialogueUI();
        }
        else
        {
            scrollRect = commentsPanel.GetComponentInParent<ScrollRect>();
            scrollRect.enabled = false;
        }
        RefreshView();
    }

    public void RefreshView()
    {
        if (isWaitingForClick) return;

        ClearOptions();

        if (story.canContinue)
        {
            ClearTextPanel();
            ProcessDialogue();
        }
        else if (story.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
        else
        {
            EndDialogue();
        }
    }

    private void ProcessDialogue()
    {
        string text = story.Continue().Trim();
        string speaker = GetSpeaker();

        UpdateSpeaker(speaker);
        isWaitingForClick = true;

        if (currentMode == DialogueMode.Regular)
        {
            CreateContentView(text, textPanel);
        }
        else
        {
            HandleCommentMode(text, speaker);
        }
    }

    private void HandleCommentMode(string text, string speaker)
    {
        if ((bool)story.variablesState["monologue"])
        {
            DialogueUIController.Instance.EnableDialogueUI();
            CreateContentView(text, textPanel);
        }
        else
        {
            DialogueUIController.Instance.DisableDialogueUI();
            CreateCommentContentView(text, speaker);
        }
    }

    private void DisplayChoices()
    {
        DialogueUIController.Instance.EnableDialogueOptions();

        foreach (Choice choice in story.currentChoices)
        {
            Button button = CreateChoiceView(choice.text.Trim());
            button.onClick.AddListener(() => OnClickChoiceButton(choice));
        }
    }

    private void EndDialogue()
    {
        DialogueUIController.Instance.DisableDialogueUI();
        DialogueManager.Instance.EndDialogue();
        if (currentMode == DialogueMode.Comments) scrollRect.enabled = true;
        currentSpeaker = null;
    }

    public string GetSpeaker()
    {
        foreach (string tag in story.currentTags)
        {
            if (tag.StartsWith("Character:")) return tag.Substring(10).Trim();
        }
        return null;
    }

    public void UpdateSpeaker(string speaker)
    {
        if (speaker == "None")
        {
            speakerText.text = " ";
            currentSpeaker = " ";
            DialogueUIController.Instance.UpdateCharacterPortrait("");
            DialogueUIController.Instance.DisableCharacterNamePanel();
        }
        else if (!string.IsNullOrEmpty(speaker))
        {
            if (currentSpeaker != speaker)
            {
                currentSpeaker = speaker;
                speakerText.text = currentSpeaker;
                if (!(DialogueMode.Comments == currentMode && !(bool)story.variablesState["monologue"]))
                {
                    DialogueUIController.Instance.EnableCharacterNamePanel();
                    DialogueUIController.Instance.UpdateCharacterPortrait(currentSpeaker);
                }
            }
        }
        else
        {
            speakerText.text = currentSpeaker ?? "";
        }
    }

    private void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        DialogueUIController.Instance.DisableDialogueOptions();
        RefreshView();
    }

    private void CreateContentView(string text, GameObject parentPanel)
    {
        TMP_Text storyText = Instantiate(textPrefab, parentPanel.transform, false);
        storyText.text = text;
    }

    private void CreateCommentContentView(string text, string speaker)
    {
        // Check if the scroll view is at the bottom
        bool isAtBottom = Mathf.Approximately(scrollRect.verticalNormalizedPosition, 0f);

        // Instantiate and set up the new comment
        GameObject newComment = Instantiate(commentPrefab, commentsPanel.transform, false);
        TMP_Text commentText = newComment.GetComponentInChildren<TMP_Text>();
        commentText.text = text;

        // Force the layout to update
        Canvas.ForceUpdateCanvases();

        // If the scroll view was at the bottom, scroll to the bottom
        if (isAtBottom)
        {
            StartCoroutine(ScrollToBottom());
        }
    }

    private IEnumerator ScrollToBottom()
    {
        // Wait until the end of the frame to ensure layout is updated
        yield return new WaitForEndOfFrame();

        // Set the scroll position to the bottom
        scrollRect.verticalNormalizedPosition = 0f;

        // Optional: Animate the scrolling
        // scrollRect.content.DOAnchorPosY(0f, 0.3f).SetEase(Ease.OutQuad);
    }


    private IEnumerator ScrollDownByCommentHeight(RectTransform commentRect)
    {
        yield return new WaitForEndOfFrame();
        float commentHeight = commentRect.rect.height * 1.25f;
        float newScrollY = scrollRect.content.anchoredPosition.y + commentHeight;
        scrollRect.content.DOAnchorPosY(newScrollY, 0.3f).SetEase(Ease.OutQuad);
    }

    private Button CreateChoiceView(string text)
    {
        Button choice = Instantiate(buttonPrefab, optionsPanel.transform, false);
        choice.GetComponentInChildren<TMP_Text>().text = text;
        return choice;
    }

    private void ClearUI()
    {
        ClearOptions();
        ClearTextPanel();
    }

    private void ClearOptions()
    {
        foreach (Transform child in optionsPanel.transform) Destroy(child.gameObject);
    }

    private void ClearTextPanel()
    {
        foreach (Transform child in textPanel.transform) Destroy(child.gameObject);
    }

    public void DisableInteractivity()
    {
        isWaitingForClick = false;
    }

    public void EnableInteractivity()
    {
        isWaitingForClick = true;
    }
}
