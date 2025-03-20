using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignalProcessor : MonoBehaviour
{
    private HashSet<string> playedCutscenes = new HashSet<string>();

    // Loads scene by name reference
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        print("Tried To load this scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Initiates Dialogue based on dialogue key, preventing replaying cutscenes
    public void InitiateDialogueSequence(string dialogueKey)
    {
        if (string.IsNullOrEmpty(dialogueKey)) return;

        if (playedCutscenes.Contains(dialogueKey))
        {
            Debug.Log("Cutscene already played: " + dialogueKey);
            return;
        }

        // Mark this cutscene as played
        playedCutscenes.Add(dialogueKey);
        DialogueManager.Instance.StartRegularDialogue(dialogueKey);
    }
}
