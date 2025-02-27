using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlSwitcher : MonoBehaviour
{
    [SerializeField] private PlayerController[] characterControllers;
    private int activeCharacterIndex = 0;

    private void Start()
    {
        SetActiveCharacter(activeCharacterIndex);
    }

    // updates the player controller that is active
    public void OnSwitchCharacter(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int nextIndex = (activeCharacterIndex + 1) % characterControllers.Length;
            Debug.Log("Switching character " + nextIndex);

            SetActiveCharacter(nextIndex);
        }
    }

    // sets the active player controller
    private void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characterControllers.Length; i++)
        {
            characterControllers[i].IsActive = (i == index);
        }

        activeCharacterIndex = index;
    }
}
