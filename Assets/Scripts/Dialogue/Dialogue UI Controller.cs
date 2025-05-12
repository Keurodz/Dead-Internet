using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;

public class DialogueUIController : MonoBehaviour
{
    // Singleton instance for easy access to DialogueUIController
    public static DialogueUIController Instance { get; private set; }

    private GameObject _DialogueOptions;        // UI panel for dialogue options
    private GameObject _TextPanel;              // UI panel for displaying text
    private GameObject _CharacterNamePanel;    // UI panel for character name
    private GameObject _PortraitHolder;        // UI panel for character portrait
    private RawImage _CharacterPortrait;       // RawImage for character portrait
    private GameObject _InteractionBlock;      // Block UI elements from interaction during dialogue

    private Dictionary<string, Texture2D> portraitDictionary;  // Store loaded portraits by name
    private Dictionary<string, Vector2> portraitSizes;         // Store portrait sizes

    void Awake()
    {
        // Singleton setup to ensure only one instance of DialogueUIController exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy if another instance already exists
            return;
        }
        Instance = this; // Set the singleton instance
        DontDestroyOnLoad(gameObject); // Persist across scene changes

        // Find child UI elements by name and assign to respective fields
        _DialogueOptions = transform.Find("Dialogue Options")?.gameObject;
        _TextPanel = transform.Find("Text Panel")?.gameObject;
        _CharacterNamePanel = transform.Find("Character Name Panel")?.gameObject;
        _PortraitHolder = transform.Find("Portrait Holder")?.gameObject;
        _CharacterPortrait = transform.Find("Portrait Holder")?.GetComponentInChildren<RawImage>();
        _InteractionBlock = transform.Find("Interaction Block")?.gameObject;

        // Check for null references and log errors if any panels are missing
        if (_DialogueOptions == null) Debug.LogError("DialogueOptions panel not found!");
        if (_TextPanel == null) Debug.LogError("TextPanel not found!");
        if (_CharacterNamePanel == null) Debug.LogError("CharacterNamePanel not found!");
        if (_CharacterPortrait == null) Debug.LogError("CharacterPortrait image not found!");

        LoadPortraits();  // Load all portraits from the Resources folder
        DisableUI();      // Disable all UI elements initially
    }

    private void Start()
    {
        DisableDialogueUI();  // Ensure dialogue UI is disabled on start
    }

    // Load portraits from Resources and store them in dictionaries
    void LoadPortraits()
    {
        portraitDictionary = new Dictionary<string, Texture2D>();
        portraitSizes = new Dictionary<string, Vector2>();
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Portraits"); // Load all portraits from Resources

        // Populate portrait dictionary and sizes
        foreach (Texture2D texture in textures)
        {
            portraitDictionary[texture.name] = texture;
            portraitSizes[texture.name] = new Vector2(texture.width, texture.height);
        }
    }

    // Update the character portrait by name and adjust size if needed
    public void UpdateCharacterPortrait(string portraitName)
    {
        if (_CharacterPortrait == null) return;

        if (!string.IsNullOrEmpty(portraitName) && portraitDictionary.ContainsKey(portraitName))
        {
            _CharacterPortrait.texture = portraitDictionary[portraitName];

            // Only adjust size if it's not a profile picture (doesn't end with "prof")
            if (portraitSizes.ContainsKey(portraitName) && !portraitName.EndsWith("prof"))
            {
                RectTransform portraitRect = _CharacterPortrait.GetComponent<RectTransform>();
                Vector2 nativeSize = portraitSizes[portraitName];
                portraitRect.sizeDelta = nativeSize; // Set the size of the portrait
            }

            _PortraitHolder.SetActive(true); // Make portrait visible
            DialogueAnimator.AnimateFadeIn(_PortraitHolder); // Animate fade-in
        }
        else
        {
            DialogueAnimator.AnimateFadeOut(_PortraitHolder); // If no portrait found, fade out
        }
    }

    // Update the character's profile picture during comment mode
    public void UpdateCharacterPortraitComment(string portraitName, RawImage portrait)
    {
        if (portrait == null) return;

        string textureKey = !string.IsNullOrEmpty(portraitName) && portraitDictionary.ContainsKey(portraitName)
            ? portraitName
            : "defaultprof"; // Default profile picture if not found

        // If the portrait is different from the texture, update it
        if (portrait.texture != portraitDictionary[textureKey])
        {
            portrait.texture = portraitDictionary[textureKey];

            // Profile images are assumed to be UI-compatible, illustrations are only slightly off in size from each other
        }
    }

    // Enable dialogue options UI panel with animation
    public void EnableDialogueOptions() => DialogueAnimator.AnimateSlideIn(_DialogueOptions);

    // Disable dialogue options UI panel with animation
    public void DisableDialogueOptions() => DialogueAnimator.AnimateSlideOut(_DialogueOptions);

    // Enable character name UI panel with animation
    public void EnableCharacterNamePanel() => DialogueAnimator.AnimateScaleIn(_CharacterNamePanel);

    // Disable character name UI panel with animation
    public void DisableCharacterNamePanel() => DialogueAnimator.AnimateScaleOut(_CharacterNamePanel);

    // Enable text panel with fade-in animation
    public void EnableTextPanel() => DialogueAnimator.AnimateFadeIn(_TextPanel);

    // Disable text panel with fade-out animation
    public void DisableTextPanel() => DialogueAnimator.AnimateFadeOut(_TextPanel);

    // Enable the full dialogue UI (name, text, portrait, etc.)
    public void EnableDialogueUI()
    {
        EnableCharacterNamePanel();
        EnableTextPanel();
        _InteractionBlock.SetActive(true); // Enable interaction block
    }

    // Disable the full dialogue UI
    public void DisableDialogueUI()
    {
        DisableCharacterNamePanel();
        DisableTextPanel();
        _InteractionBlock.SetActive(false); // Disable interaction block
        DialogueAnimator.AnimateFadeOut(_PortraitHolder); // Fade out portrait
    }

    // Disable all UI elements at once
    public void DisableUI()
    {
        _CharacterNamePanel?.SetActive(false);
        _TextPanel?.SetActive(false);
        _DialogueOptions?.SetActive(false);
        _PortraitHolder?.SetActive(false);
    }

    // Enable the portrait holder and animate fade-in
    public void EnablePortrait()
    {
        _PortraitHolder.SetActive(true);
        DialogueAnimator.AnimateFadeIn(_PortraitHolder);
    }
}
