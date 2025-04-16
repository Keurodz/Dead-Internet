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

    public enum DialogueMode { Regular, Comments, Chat }

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
    private bool isProcessingExternalFunction = false;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip commentPopSound;
    [SerializeField] private AudioClip mouseClick;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(mouseClick);

            if (!inCutscene && !isProcessingExternalFunction)
            {
                TypewriterEffectDOTween activeTypewriter = textPanel.GetComponentInChildren<TypewriterEffectDOTween>();

                if (activeTypewriter != null && activeTypewriter.IsTyping())
                {
                    activeTypewriter.SkipTyping();
                }
                else if (isWaitingForClick)
                {
                    isWaitingForClick = false;
                    RefreshView();
                }
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

        // Register observers for external function execution
        RegisterExternalFunctionObservers();

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

    private void RegisterExternalFunctionObservers()
    {
        // Add observers for each external function to detect when they're being called
        this.story.ObserveVariable("monologue", (string varName, object newValue) => {
            // Do nothing, this is just to track regular variables
        });

        // Observe when a function is about to be called
        this.story.onEvaluateFunction += HandleFunctionEvaluation;
    }

    private void HandleFunctionEvaluation(string functionName, object[] arguments)
    {
        // These are the external functions we want to handle specially
        if (functionName == "PLAY_CUTSCENE" ||
            functionName == "PLAY_MUSIC" ||
            functionName == "STOP_MUSIC" ||
            functionName == "PLAY_CUTSCENE_WITH_MUSIC")
        {
            isProcessingExternalFunction = true;

            // We're going to process the function and then continue without requiring a click
            StartCoroutine(ContinueAfterExternalFunction());
        }
    }

    private IEnumerator ContinueAfterExternalFunction()
    {
        // Wait a short time to ensure the external function completes
        yield return new WaitForSeconds(0.1f);

        // If we were waiting for a click, we no longer need to
        if (isWaitingForClick)
        {
            isWaitingForClick = false;

            // Continue processing the story
            RefreshView();
        }

        isProcessingExternalFunction = false;
    }

    public void RefreshView()
    {
        if (isWaitingForClick && !isProcessingExternalFunction) return;

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

        // Check if this line is just a function call with no visible text
        if (string.IsNullOrWhiteSpace(text))
        {
            // If it's an empty line (just a function call), don't wait for click
            isWaitingForClick = false;
            RefreshView();
            return;
        }

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
        // Clean up our observer before ending
        if (story != null)
        {
            story.onEvaluateFunction -= HandleFunctionEvaluation;
        }

        DialogueUIController.Instance.DisableDialogueUI();
        DialogueManager.Instance.EndDialogue();
        if (currentMode == DialogueMode.Comments || currentMode == DialogueMode.Chat) scrollRect.enabled = true;
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
                if (!((DialogueMode.Chat == currentMode || DialogueMode.Comments == currentMode) && !(bool)story.variablesState["monologue"]))
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
        TypewriterEffectDOTween typewriter = storyText.GetComponent<TypewriterEffectDOTween>();
        typewriter.StartTyping(text);
    }

    private void CreateCommentContentView(string text, string speaker)
    {
        bool isAtBottom = Mathf.Approximately(scrollRect.verticalNormalizedPosition, 0f);

        GameObject newComment = Instantiate(commentPrefab, commentsPanel.transform, false);
        TMP_Text commentText = newComment.GetComponentInChildren<TMP_Text>();
        commentText.text = text;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)commentsPanel.transform);

        DialogueAnimator.AnimateSlideIn(newComment);
        DialogueUIController.Instance.UpdateCharacterPortraitComment(speaker + "prof", newComment.GetComponentInChildren<RawImage>());

        if (commentPopSound && audioSource)
        {
            audioSource.pitch = 0.7f;
            audioSource.PlayOneShot(commentPopSound);
        }

        if (currentMode == DialogueMode.Chat)
        {
            DOTween.To(() => scrollRect.verticalNormalizedPosition, x => scrollRect.verticalNormalizedPosition = x, 0f, 0.3f).SetEase(Ease.OutQuad);
        }
        else
        {
            StartCoroutine(ScrollDownByCommentHeight(newComment.GetComponent<RectTransform>()));
        }
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

    public void ScrollToElement(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // Calculate the position of the target relative to the content
        Vector2 targetPosition = (Vector2)content.InverseTransformPoint(content.position) - (Vector2)content.InverseTransformPoint(target.position);

        // Calculate the normalized position
        float normalizedPosition = 1 - (targetPosition.y / (content.rect.height - viewport.rect.height));

        // Clamp the normalized position between 0 and 1
        normalizedPosition = Mathf.Clamp01(normalizedPosition);

        // Set the scroll position
        scrollRect.verticalNormalizedPosition = normalizedPosition;
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