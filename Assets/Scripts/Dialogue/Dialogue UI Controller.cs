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
        DisableDialogueUI();
    }

    void LoadPortraits()
    {
        portraitDictionary = new Dictionary<string, Texture2D>();
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Portraits");

        print("Load Portraits tried to run ");
        foreach (Texture2D sprite in textures)
        {
            print("Portrait: " + sprite.name);
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
        }
        else
        {
            _PortraitHolder.SetActive(false); // Hide if no valid portrait
        }
    }


public void DisableDialogueOptions()
    {
        if (_DialogueOptions != null) _DialogueOptions.SetActive(false);
    }

    public void DisableCharacterNamePanel()
    {
        if (_CharacterNamePanel != null) _CharacterNamePanel.SetActive(false);
    }

    public void DisableTextPanel()
    {
        if (_TextPanel != null) _TextPanel.SetActive(false);
    }

    public void EnableCharacterPortrait()
    {
        if (_CharacterPortrait != null) _PortraitHolder.SetActive(true);
    }

    public void DisableCharacterPortrait()
    {
        if (_CharacterPortrait != null) _PortraitHolder.SetActive(false);
    }

    public void EnableDialogueOptions()
    {
        if (_DialogueOptions != null) _DialogueOptions.SetActive(true);
    }

    public void EnableCharacterNamePanel()
    {
        if (_CharacterNamePanel != null) _CharacterNamePanel.SetActive(true);
    }

    public void EnableTextPanel()
    {
        if (_TextPanel != null) _TextPanel.SetActive(true);
    }

    public void DisableDialogueUI()
    {
        if (_CharacterNamePanel != null) _CharacterNamePanel.SetActive(false);
        if (_DialogueOptions != null) _DialogueOptions.SetActive(false);
        if (_TextPanel != null) _TextPanel.SetActive(false);
        if (_PortraitHolder != null) _PortraitHolder.SetActive(false);
    }

    public void EnableDialogueUI()
    {
        if (_CharacterNamePanel != null) _CharacterNamePanel.SetActive(true);
        if (_TextPanel != null) _TextPanel.SetActive(true);
    }
}
