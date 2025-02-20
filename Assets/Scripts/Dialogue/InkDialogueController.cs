using UnityEngine;
using Ink.Runtime;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Import DoTween
using System.Collections;

public class InkDialogueController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    public enum DialogueMode
    {
        Regular,
        Comments
    }

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
    private GameObject commentPrefab;

    [SerializeField]
    private GameObject textPanel = null; // For regular dialogue

    [SerializeField]
    private GameObject commentsPanel = null; // For comment-style dialogue

    [SerializeField]
    private TMP_Text speakerText; // For regular dialogue

    private bool isWaitingForClick = false;
    private string currentSpeaker = null;

    private DialogueMode currentMode = DialogueMode.Regular; // Default mode

    private ScrollRect scrollRect; // Scroll pane when in comment mode 

    void Update()
    {
        if (isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            isWaitingForClick = false;
            RefreshView();
        }
    }

    public void InitiateDialogue(Story story, DialogueMode mode = DialogueMode.Regular)
    {
        this.story = story;
        this.currentMode = mode; // Set the dialogue mode
        ClearOptions();
        ClearTextPanel();
        if (DialogueMode.Regular == mode)
        {
            DialogueUIController.Instance.EnableDialogueUI();
        }
        else
        {
            scrollRect = commentsPanel.GetComponentInParent<ScrollRect>();
            scrollRect.GetComponent<ScrollRect>().enabled = false;
        }
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

            if (currentMode == DialogueMode.Regular)
            {
                if (speaker == "None")
                {
                    speakerText.text = " ";
                    currentSpeaker = " ";
                    DialogueUIController.Instance.UpdateCharacterPortrait("");
                }
                else if (!string.IsNullOrEmpty(speaker))
                {
                    currentSpeaker = speaker;
                    speakerText.text = currentSpeaker;
                    DialogueUIController.Instance.UpdateCharacterPortrait(currentSpeaker);
                }
                else if (currentSpeaker != null)
                {
                    speakerText.text = currentSpeaker;
                }
                else
                {
                    speakerText.text = "";
                    currentSpeaker = null;
                    DialogueUIController.Instance.UpdateCharacterPortrait("");
                }

                CreateContentView(text, textPanel);
                isWaitingForClick = true;
            }
            else if (currentMode == DialogueMode.Comments)
            {
                // For comment-style dialogue, add the text to the comments panel
                CreateCommentContentView(text, commentsPanel, speaker);
                isWaitingForClick = true;
            }
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
            DialogueUIController.Instance.DisableDialogueUI();

            if (currentMode == DialogueMode.Comments)
            {
                scrollRect.GetComponent<ScrollRect>().enabled = true;
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
        story.ChooseChoiceIndex(choice.index);
        DialogueUIController.Instance.DisableDialogueOptions();
        RefreshView();
    }

    void CreateContentView(string text, GameObject parentPanel)
    {
        TMP_Text storyText = Instantiate(textPrefab) as TMP_Text;
        storyText.text = text;
        storyText.transform.SetParent(parentPanel.transform, false);
    }

    void CreateCommentContentView(string text, GameObject parentPanel, string speaker)
    {
        // Instantiate the comment prefab inside the parent panel
        GameObject newComment = Instantiate(commentPrefab, parentPanel.transform, false);
        TMP_Text commentText = newComment.GetComponentInChildren<TMP_Text>();
        commentText.text = text;

        // Ensure proper scaling
        newComment.transform.localScale = Vector3.one;

        // Get the RectTransform of the new comment
        RectTransform commentRect = newComment.GetComponent<RectTransform>();

        // Force layout update to ensure correct positioning before animation
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentPanel.GetComponent<RectTransform>());

        // Animate the comment appearance
        DialogueAnimator.AnimateCommentSlideIn(commentRect);
        RawImage im = newComment.GetComponentInChildren<RawImage>();
        DialogueUIController.Instance.UpdateCharacterPortraitComment(speaker + "prof", im);

        // Scroll down by the height of the new comment
        StartCoroutine(ScrollDownByCommentHeight(commentRect, parentPanel));
    }

    IEnumerator ScrollDownByCommentHeight(RectTransform commentRect, GameObject parentPanel)
    {
        yield return new WaitForEndOfFrame(); // Ensure layout updates before measuring

        ScrollRect scrollRect = parentPanel.GetComponentInParent<ScrollRect>();

        if (scrollRect != null)
        {
            float commentHeight = commentRect.rect.height * 1.25f; // Get the height of the new comment
            float newScrollY = scrollRect.content.anchoredPosition.y + commentHeight;

            // Smoothly scroll to the new position
            scrollRect.content.DOAnchorPosY(newScrollY, 0.3f).SetEase(Ease.OutQuad);
        }
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