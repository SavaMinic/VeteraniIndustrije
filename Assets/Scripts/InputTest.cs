using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    public InputActionAsset inputActionsAsset;

    Vector2 moveInput;
    bool interactInput;

    InputAction moveLeftAction;
    InputAction moveRightAction;
    InputAction moveUpAction;
    InputAction moveDownAction;
    InputAction interactAction;

    public Transform inputPreview;

    private void Awake()
    {
        var playerActionmap = inputActionsAsset.actionMaps[0]; // player 1
        moveLeftAction = playerActionmap.FindAction("Move Left");
        moveRightAction = playerActionmap.FindAction("Move Right");
        moveUpAction = playerActionmap.FindAction("Move Up");
        moveDownAction = playerActionmap.FindAction("Move Down");
        interactAction = playerActionmap.FindAction("Interact");

        inputActionsAsset.Enable();
    }

    bool bindingInProgress;

    private void Update()
    {
        //moveInput = moveLeftAction.ReadValue<Vector2>();
        interactInput = interactAction.triggered;

        float x = moveLeftAction.ReadValue<float>() == 1 ? -1 : moveRightAction.ReadValue<float>() == 1 ? 1 : 0;
        float y = moveDownAction.ReadValue<float>() == 1 ? -1 : moveUpAction.ReadValue<float>() == 1 ? 1 : 0;
        moveInput = new Vector2(x, y);


        if (interactInput)
            Debug.Log("Interacted");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartRebindDirection(0, 0);
        }

        if (inputPreview)
        {
            inputPreview.position = new Vector3(moveInput.x, moveInput.y);
        }
    }

    /*
    void StartRebindInteract(int player)
    {
        if (!bindingInProgress)
        {
            Debug.Log("Started rebinding");

            inputActionsAsset.Disable();

            interactAction.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnCancel(RebindCancel)
                .OnApplyBinding((op, keyPath) =>
                {
                    op.action.ApplyBindingOverride(0, keyPath);
                    Debug.Log("Bindings " + op.action.bindings[0]);
                })
                .OnComplete(op =>
                {
                    op.Dispose();
                    Debug.Log("Binding complete");
                    interactAction.Enable();
                    bindingInProgress = false;
                })
                .Start();

            bindingInProgress = true;
        }
    }*/

    void StartRebindDirection(int player, int dir)
    {

        if (!bindingInProgress)
        {
            var action =
                dir == 0 ? moveLeftAction :
                dir == 1 ? moveRightAction :
                dir == 2 ? moveUpAction :
                dir == 3 ? moveDownAction :
                interactAction;

            Debug.Log("Started rebinding " + action.name);

            action.Disable();

            action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnCancel(RebindCancel)
                .OnApplyBinding((op, keyPath) =>
                {
                    Debug.Log("End binding");
                    action.ApplyBindingOverride(0, keyPath);
                    Debug.Log("Bindings " + op.action.bindings[0]);
                })
                .OnComplete(op =>
                {
                    op.Dispose();
                    Debug.Log("Binding complete");
                    action.Enable();
                    bindingInProgress = false;
                })
                .Start();

            bindingInProgress = true;
        }
    }

    private void RebindCancel(InputActionRebindingExtensions.RebindingOperation obj)
    {
        Debug.Log("Canceled binding");
        bindingInProgress = false;
    }

    private void RebindSuccess(InputActionRebindingExtensions.RebindingOperation rebindingOp, string keyPath)
    {


        Debug.Log("Changed binding to " + keyPath);
        Debug.Log("Bindings " + rebindingOp.action.bindings[0]);
        inputActionsAsset.Enable();
    }
}
