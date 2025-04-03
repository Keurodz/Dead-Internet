using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// https://www.youtube.com/watch?v=xF19LIYfUmY
public class PlayerController : MonoBehaviour
{
    // is the player controller active?
    public bool IsActive { get; set; } = false;

    [SerializeField] 
    public float speed;
    // https://www.youtube.com/watch?v=HJkHnkS6z1I
    // sokoban behavior
    [SerializeField] 
    private LayerMask blockingLayer;
    [SerializeField] 
    private Animator animator;

    private Vector2 move;
    private CharacterController controller;

    // instance of the dialogue manager
    private DialogueManager dialogueManager;

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsActive) {
            move = context.ReadValue<Vector2>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        dialogueManager = DialogueManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {        
        if (IsActive == true && dialogueManager.isDialogueActive == false) {
            MovePlayer();
        } else {
            StopPlayer();
        }
    }

    private void StopPlayer() {
        move = Vector2.zero;
        animator.SetBool("pushing", false);
        animator.SetFloat("speed", move.magnitude);
    }
    
    private void MovePlayer() {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (move.magnitude == 0) {
            StopPlayer();
            return;
        } 
        
        animator.SetFloat("speed", move.magnitude);
 

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.height / 4);
        Debug.DrawRay(rayOrigin, movement.normalized * 1f, Color.red, 1f);

        bool isPushing = Physics.Raycast(rayOrigin, movement, out RaycastHit hit, 0.3f, blockingLayer);

        if (isPushing == false || movement.magnitude == 0) {
            animator.SetBool("pushing", false);
        } else {
            if (movement.magnitude > 0) {
                animator.SetBool("pushing", true);
            }
            if (hit.collider.CompareTag("Sokoban"))
            {
                var gridBlock = hit.collider.GetComponent<ISokobanInteractable>();
                if (gridBlock != null && gridBlock.IsPushable())
                {
                    Direction direction = GetCardinalDirection(hit.collider.transform.position - transform.position);
                    if (gridBlock.TryPush(direction))
                    {
                        controller.Move(movement * speed * Time.deltaTime);
                    }
                }
            }
        }

        controller.Move(movement * speed * Time.deltaTime);


        // if (!Physics.Raycast(rayOrigin, movement, out RaycastHit hit, 0.3f, blockingLayer))
        // {
        //     controller.Move(movement * speed * Time.deltaTime);
        // }
        // else if (hit.collider.CompareTag("Sokoban"))
        // {   
        //     if (movement.magnitude > 0) {
        //         animator.SetBool("pushing", true);
        //     } 
            
        //     var gridBlock = hit.collider.GetComponent<ISokobanInteractable>();

        //     if (gridBlock != null && gridBlock.IsPushable())
        //     {
        //         // difference between the player and the block
        //         Direction direction = GetCardinalDirection(hit.collider.transform.position - transform.position);
        //         if (gridBlock.TryPush(direction))
        //         {
        //             controller.Move(movement * speed * Time.deltaTime);
        //         }
        //     }
        // }
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
}
