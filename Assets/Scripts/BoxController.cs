using UnityEngine;

public class BoxController : MonoBehaviour
{
    public LayerMask blockingLayer;

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TryToPushBox(Vector3 movement, float moveSpeed)
    {
        var targetPosition = transform.position + movement;
        
        if (Physics.Raycast(transform.position, movement, out RaycastHit hit, 1f, blockingLayer))
        {
            return false;
        }
        else
        {
            transform.Translate(movement, Space.World);
            return true;
        }
    }
}
