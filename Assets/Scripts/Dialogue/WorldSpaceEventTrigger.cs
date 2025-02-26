using UnityEngine;

public class WorldSpaceEventTrigger : MonoBehaviour
{
    public string eventID = "";
    public bool dialogueMode = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (dialogueMode)
            {
                DialogueManager.Instance.StartRegularDialogue(eventID);
            }
            else
            {
                TimelineDialogueManager.Instance.PlayTriggerCutscene(eventID);
            }
            
        }
    }
}
