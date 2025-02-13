using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    [SerializeField] 
    public float distance;

    void Update() {
        if (Vector3.Distance(transform.position, player.transform.position) > distance) {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime);
        }
    }
}
