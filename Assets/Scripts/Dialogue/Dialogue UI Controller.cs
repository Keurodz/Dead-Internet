using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUIController : MonoBehaviour
{
    // Singleton instance
    public static DialogueUIController Instance { get; private set; }

    private GameObject _DialogueOptions;
    private GameObject _TextPanel;
    private GameObject _CharacterNamePanel;
    private GameObject _PortraitHolder;
    private RawImage _CharacterPortrait;

    private Dictionary<string, Texture2D> portraitDictionary;
    
    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Keeps the instance persistent across scenes

        // Automatically find child objects by name
        _DialogueOptions = transform.Find("Dialogue Options")?.gameObject;
        _TextPanel = transform.Find("Text Panel")?.gameObject;
        _CharacterNamePanel = transform.Find("Character Name Panel")?.gameObject;
        _PortraitHolder = transform.Find("Portrait Holder")?.gameObject;
        _CharacterPortrait = transform.Find("Portrait Holder")?.GetComponentInChildren<RawImage>();

        // Check for null references and log errors if any panels are missing
        if (_DialogueOptions == null) Debug.LogError("DialogueOptions panel not found!");
        if (_TextPanel == null) Debug.LogError("TextPanel not found!");
        if (_CharacterNamePanel == null) Debug.LogError("CharacterNamePanel not found!");
        if (_CharacterPortrait == null) Debug.LogError("CharacterPortrait image not found!");

        LoadPortraits();
        DisableUI();
    }

    private void Start()
    {
        DisableDialogueUI();
    }

    void LoadPortraits()
    {
        portraitDictionary = new Dictionary<string, Texture2D>();
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Portraits");

        foreach (Texture2D sprite in textures)
        {
            portraitDictionary[sprite.name] = sprite;
        }
    }

    public void UpdateCharacterPortrait(string portraitName)
    {
        if (_CharacterPortrait == null) return;

        if (!string.IsNullOrEmpty(portraitName) && portraitDictionary.ContainsKey(portraitName))
        {
            _CharacterPortrait.texture = portraitDictionary[portraitName];
            _PortraitHolder.SetActive(true);
            DialogueAnimator.AnimateFadeIn(_PortraitHolder);
        }
        else
        {
            DialogueAnimator.AnimateFadeOut(_PortraitHolder);
        }
    }

    public void UpdateCharacterPortraitComment(string portraitName, RawImage portrait)
    {
        if (portrait == null) return;

        if (!string.IsNullOrEmpty(portraitName) && portraitDictionary.ContainsKey(portraitName))
        {
            if (portrait.texture != portraitDictionary[portraitName]) 
            {
                portrait.texture = portraitDictionary[portraitName];
            }
        }
        else
        {
            if (portrait.texture != portraitDictionary["defaultprof"]) 
            {
                portrait.texture = portraitDictionary["defaultprof"];
            }
        }
    }



    public void EnableDialogueOptions() => DialogueAnimator.AnimateSlideIn(_DialogueOptions);
    public void DisableDialogueOptions() => DialogueAnimator.AnimateSlideOut(_DialogueOptions);

    public void EnableCharacterNamePanel() => DialogueAnimator.AnimateScaleIn(_CharacterNamePanel);
    public void DisableCharacterNamePanel() => DialogueAnimator.AnimateScaleOut(_CharacterNamePanel);

    public void EnableTextPanel() => DialogueAnimator.AnimateFadeIn(_TextPanel);
    public void DisableTextPanel() => DialogueAnimator.AnimateFadeOut(_TextPanel);

    public void EnableDialogueUI()
    {
        EnableCharacterNamePanel();
        EnableTextPanel();
    }

    public void DisableDialogueUI()
    {
        DisableCharacterNamePanel();
        DisableTextPanel();
        //DisableDialogueOptions();
        DialogueAnimator.AnimateFadeOut(_PortraitHolder);
    }

    public void DisableUI()
    {
        _CharacterNamePanel?.SetActive(false);
        _TextPanel?.SetActive(false);
        _DialogueOptions?.SetActive(false);
        _PortraitHolder?.SetActive(false);
    }

    public void EnablePortrait()
    {
        _PortraitHolder.SetActive(true);
        DialogueAnimator.AnimateFadeIn(_PortraitHolder);
    }
}

