using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Rebinder : MonoBehaviour
{
    public InputActionAsset inputActionsAsset;

    Vector2 moveInput;
    bool interactInput;

    public class PlayerInputAction
    {
        public InputAction moveLeftAction;
        public InputAction moveRightAction;
        public InputAction moveUpAction;
        public InputAction moveDownAction;
        public InputAction interactAction;
    }

    public PlayerInputAction p1;
    public PlayerInputAction p2;

    public Transform inputPreview;

    private void Awake()
    {
        var playerActionmap = inputActionsAsset.actionMaps[0]; // player 1
        p1 = new PlayerInputAction();
        p1.moveLeftAction = playerActionmap.FindAction("Move Left");
        p1.moveRightAction = playerActionmap.FindAction("Move Right");
        p1.moveUpAction = playerActionmap.FindAction("Move Up");
        p1.moveDownAction = playerActionmap.FindAction("Move Down");
        p1.interactAction = playerActionmap.FindAction("Interact");

        playerActionmap = inputActionsAsset.actionMaps[1]; // player 2
        p2 = new PlayerInputAction();
        p2.moveLeftAction = playerActionmap.FindAction("Move Left");
        p2.moveRightAction = playerActionmap.FindAction("Move Right");
        p2.moveUpAction = playerActionmap.FindAction("Move Up");
        p2.moveDownAction = playerActionmap.FindAction("Move Down");
        p2.interactAction = playerActionmap.FindAction("Interact");
    }

    public bool bindingInProgress { get; private set; }
    public delegate void BindingComplete();
    public event BindingComplete OnBindingComplete;

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            PlayerInputAction p = i == 0 ? p1 : p2;

            //moveInput = moveLeftAction.ReadValue<Vector2>();
            interactInput = p.interactAction.triggered;

            float x = p.moveLeftAction.ReadValue<float>() == 1 ? -1 : p.moveRightAction.ReadValue<float>() == 1 ? 1 : 0;
            float y = p.moveDownAction.ReadValue<float>() == 1 ? -1 : p.moveUpAction.ReadValue<float>() == 1 ? 1 : 0;
            moveInput = new Vector2(x, y);

            if (interactInput)
                Debug.Log("Interacted");
        }

        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartRebindDirection(0, 0);
        }*/

        if (inputPreview)
        {
            inputPreview.position = new Vector3(moveInput.x, moveInput.y);
        }
    }

    public void StartRebindDirection(int player, int dir)
    {
        PlayerInputAction p = player == 0 ? p1 : p2;

        if (!bindingInProgress)
        {
            var action =
                dir == 0 ? p.moveLeftAction :
                dir == 1 ? p.moveRightAction :
                dir == 2 ? p.moveUpAction :
                dir == 3 ? p.moveDownAction :
                p.interactAction;

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
                    OnBindingComplete?.Invoke();
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
