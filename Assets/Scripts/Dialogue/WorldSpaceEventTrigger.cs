using UnityEngine;

public class WorldSpaceEventTrigger : MonoBehaviour
{
    public string eventID = "";
    public bool dialogueMode = true;

    private bool exhausted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!exhausted)
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
                exhausted = true;

            }
        }
    }
}
