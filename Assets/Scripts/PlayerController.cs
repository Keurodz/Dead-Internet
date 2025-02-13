using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// https://www.youtube.com/watch?v=xF19LIYfUmY
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed;
    // https://www.youtube.com/watch?v=HJkHnkS6z1I
    // sokoban behavior
    [SerializeField] public LayerMask blockingLayer;
    [SerializeField] public Animator animator;

    private Vector2 move;
    private CharacterController controller;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
    }

    public void movePlayer() {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        if (movement.magnitude == 0) {
            animator.SetBool("walking", false);
            animator.SetBool("pushing", false);

            // Debug.Log("Not Walking");
            return;
        } else {
            animator.SetBool("walking", true);
            // Debug.Log("Walking");
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        Vector3 rayOrigin = controller.transform.position + Vector3.up * (controller.height / 4);
        Debug.DrawRay(rayOrigin, movement.normalized * 1f, Color.red, 1f);
        Direction direction = GetCardinalDirection(movement);

        if (!Physics.Raycast(rayOrigin, movement, out RaycastHit hit, 0.5f, blockingLayer))
        {
            controller.Move(movement * speed * Time.deltaTime);
            // transform.Translate(movement * speed * Time.deltaTime, Space.World);
            Debug.Log("No hit");
        }
        else if (hit.collider.CompareTag("Sokoban"))
        {
            Debug.Log("Hit Sokoban");
            if (!animator.GetBool("pushing")) 
            {
                animator.SetBool("pushing", true);
            }
            var gridBlock = hit.collider.GetComponent<ISokobanInteractable>();

            if (gridBlock != null && gridBlock.IsPushable())
            {
                if (gridBlock.TryPush(direction))
                {
                    controller.Move(movement * speed * Time.deltaTime);
                    // transform.Translate(movement * speed * Time.deltaTime, Space.World);
                }
            }
        } else {
            Debug.Log("Hit something else");
        }
    }

    // gets the cardinal direction of the given movement vector
    private Direction GetCardinalDirection(Vector3 movement)
    {
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.z))
        {
            return movement.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            return movement.z > 0 ? Direction.Up : Direction.Down;
        }
    }

    private IEnumerator PushAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("pushing", false);
    }
}
