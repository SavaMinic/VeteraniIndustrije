using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Rebinder : MonoBehaviour
{


    Vector2 moveInput;
    bool interactInput;

    public Transform inputPreview;

    private void Awake()
    {

    }

    public bool bindingInProgress { get; private set; }
    public delegate void BindingComplete();
    public event BindingComplete OnBindingComplete;

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            DomacinInputManager.Actions p = i == 0 ?
                DomacinInputManager.e.p1 :
                DomacinInputManager.e.p2;

            //moveInput = moveLeftAction.ReadValue<Vector2>();
            interactInput = p.interactAction.triggered;

            float x = p.moveLeftAction.ReadValue<float>() == 1 ? -1 : p.moveRightAction.ReadValue<float>() == 1 ? 1 : 0;
            float y = p.moveDownAction.ReadValue<float>() == 1 ? -1 : p.moveUpAction.ReadValue<float>() == 1 ? 1 : 0;
            moveInput = new Vector2(x, y);

            //if (interactInput)
            //Debug.Log("Interacted");
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
        DomacinInputManager.Actions p = player == 0 ?
            DomacinInputManager.e.p1 :
            DomacinInputManager.e.p2;

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
                .OnCancel((op) =>
                {
                    op.Dispose();
                    Debug.Log("Canceled binding");
                    bindingInProgress = false;
                    action.Enable();

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

        OnBindingComplete?.Invoke();
    }

    private void RebindSuccess(InputActionRebindingExtensions.RebindingOperation rebindingOp, string keyPath)
    {
        Debug.Log("Changed binding to " + keyPath);
        Debug.Log("Bindings " + rebindingOp.action.bindings[0]);
        DomacinInputManager.e.inputActionsAsset.Enable();
    }
}
