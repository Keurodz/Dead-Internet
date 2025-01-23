using UnityEngine;
using UnityEngine.InputSystem;

// https://www.youtube.com/watch?v=xF19LIYfUmY
public class PlayerController : MonoBehaviour
{
    public float speed;
    // https://www.youtube.com/watch?v=HJkHnkS6z1I
    // sokoban behavior
    public LayerMask blockingLayer;
    private Vector2 move;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
    }

    public void movePlayer() {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        if (movement.magnitude == 0) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        Debug.DrawRay(transform.position, movement.normalized * 1f, Color.red, 1f);

        if (!Physics.Raycast(transform.position, movement, out RaycastHit hit, 1f, blockingLayer))
        {
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
        }
        else if (hit.collider.CompareTag("Box"))
        {
            
            Debug.Log("Box");
            var box = hit.collider.GetComponent<BoxController>();
            if (box != null && box.TryToPushBox(movement, speed)) {
                transform.Translate(movement * speed * Time.deltaTime, Space.World);
            }
        }
    }
}
