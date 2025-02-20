using UnityEngine;
using System.Collections.Generic;

// This script can be attached to a game object to make it follow the player.
// The game object will follow the grid system to get to the player.
public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    [SerializeField] 
    public float distance;

    private SokobanGridSystem gridSystem;

    private List<Vector2Int> pathToPlayer = new List<Vector2Int>();
    private int currentPathIndex = 0;
    public float moveSpeed = 2f; 

    void Start() {
        gridSystem = FindFirstObjectByType<SokobanGridSystem>();
    }

    void Update() {
        if (pathToPlayer.Count > 0 && currentPathIndex < pathToPlayer.Count) {
            Vector3 targetPosition = gridSystem.GetWorldPosition(pathToPlayer[currentPathIndex]);
            
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Check if reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
                currentPathIndex++;
            }
        } else if (Vector3.Distance(transform.position, player.transform.position) > distance) {
            // Generate a new path to the player if needed
            Vector2Int thisPosition = gridSystem.GetGridPosition(transform.position);
            Vector2Int playerPosition = gridSystem.GetGridPosition(player.transform.position);
            pathToPlayer = GetPath(thisPosition, playerPosition);
            currentPathIndex = 0;
        }
    }

    // gets the path from this object to the player
    private List<Vector2Int> GetPath(Vector2Int thisPosition, Vector2Int playerPosition) {
        return gridSystem.GetPath(thisPosition, playerPosition);
    }
}
