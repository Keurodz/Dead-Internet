using UnityEngine;
using UnityEngine.SceneManagement;

public class SignalProcessor : MonoBehaviour
{
    // Loads scene by name reference
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }

        print("Tried To load this shit");
        SceneManager.LoadScene(sceneName);
    }

    // Initiates Dialogue based on dialogue key
    public void InitiateDialogueSequence(string dialogueKey)
    {
        if (!string.IsNullOrEmpty(dialogueKey))
        {
            DialogueManager.Instance.StartRegularDialogue(dialogueKey);
        }
    }
}
