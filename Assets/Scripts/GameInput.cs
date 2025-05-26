using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    // Event to notify when the player interacts with something
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnNextAction;
    public event EventHandler OnPreviousAction;
    // We inject the player input actions
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // Create the player input actions
        playerInputActions = new PlayerInputActions();
        // Enable the player input actions
        playerInputActions.Enable();

        // Add the interact event to the interact action
        // += is used to add a new method to the event
        // Because it's a callback we can call the method directly after the +=
        // playerInputActions.Player.Interact.performed += ctx => { Debug.Log("Interacted with the counter"); };
        // Also we can define the method outside and call it's signature or definition as this example 

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Next.performed += Next_performed;
        playerInputActions.Player.NextScroll.performed += Scroll_Performed;
        playerInputActions.Player.Previous.performed += Previous_performed;
        playerInputActions.Player.Exit.performed += ctx => { Application.Quit(); };

    }

    private void Scroll_Performed(InputAction.CallbackContext context)
    {

        float scrollInput = context.ReadValue<float>();
        if (scrollInput > 0)
        {
            OnNextAction?.Invoke(this, EventArgs.Empty);
        }
        if (scrollInput < 0)
        {
            OnPreviousAction?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Previous_performed(InputAction.CallbackContext context)
    {
        OnPreviousAction?.Invoke(this, EventArgs.Empty);
    }

    private void Next_performed(InputAction.CallbackContext context)
    {
        OnNextAction?.Invoke(this, EventArgs.Empty);
    }

    // this is the method that is going to happen when the interact action is performed
    // it's parameters should look like this one
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Interacted with the counter");
        OnInteractAction?.Invoke(this, EventArgs.Empty);

        // We are saynig here basically that in this interaction
        // We are the object that it's calling it
        // And we are not sending any arguments
    }

    private void InteractAlternate_performed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        // Get the input from the player with the new input system
        Vector2 input = playerInputActions.Player.Move.ReadValue<Vector2>();
        input = input.normalized;
        return input;
    }
}
