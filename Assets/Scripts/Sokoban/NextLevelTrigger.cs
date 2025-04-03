using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    ILevelController levelController;

    private void Start() {
        levelController = SokobanDungeonManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered is the player
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.IsActive = false;
            this.levelController.NextLevel();
            // DungeonSceneController.Instance.NextLevel();
            // SokobanDungeonManager.Instance.NextLevel();
        }
    }
}
