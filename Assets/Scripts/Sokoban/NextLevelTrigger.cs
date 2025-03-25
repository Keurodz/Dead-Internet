using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered is the player
        if (other.CompareTag("Player"))
        {
            DungeonSceneController.Instance.NextLevel();
        }
    }
}
