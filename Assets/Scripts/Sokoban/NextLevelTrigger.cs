using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered is the player
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.IsActive = false;

            Debug.Log("Next level trigger entered");
            Debug.Log(playerController);
            Debug.Log(playerController.IsActive);
            DungeonSceneController.Instance.NextLevel();
        }
    }
}
