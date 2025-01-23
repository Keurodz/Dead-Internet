using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    // Singleton instance
    public static DialogueUIController Instance { get; private set; }

    private GameObject _DialogueOptions;
    private GameObject _TextPanel;
    private GameObject _CharacterNamePanel;

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

        // Check for null references and log errors if any panels are missing
        if (_DialogueOptions == null) Debug.LogError("DialogueOptions panel not found!");
        if (_TextPanel == null) Debug.LogError("TextPanel not found!");
        if (_CharacterNamePanel == null) Debug.LogError("CharacterNamePanel not found!");

        DisableDialogueUI();
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
        DisableCharacterNamePanel();
        DisableDialogueOptions();
        DisableTextPanel();
    }

    public void EnableDialogueUI()
    {
        EnableCharacterNamePanel();
        EnableTextPanel();
    }
}
