﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class MainPlayer : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public int player = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        DomacinInputManager.e.inputActionsAsset.Enable();
    }

    Vector3 lastPos;

    void FixedUpdate()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        DomacinInputManager.Actions p = player == 1 ?
            DomacinInputManager.e.p1 :
            DomacinInputManager.e.p2;

        float deadzone = 0.1f;

        float left = p.moveLeftAction.ReadValue<float>();
        float right = p.moveRightAction.ReadValue<float>();
        float down = p.moveDownAction.ReadValue<float>();
        float up = p.moveUpAction.ReadValue<float>();

        float h =
            left > deadzone ? -left :
            right > deadzone ? right : 0;

        float v =
            down > deadzone ? -down :
            up > deadzone ? up : 0;

        //float h = Input.GetAxis(HorizontalAxis);
        //float v = Input.GetAxis(VerticalAxis);

        Vector3 diffPos = lastPos - transform.position;
        float diff = diffPos.magnitude;

        bool walkingInput = Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0;
        bool speedThres = diff > 0.05f;

        if (Mathf.Abs(h) > 0)
        {
            spriteRenderer.flipX = h > 0f;
        }
        animator.SetBool("walking", walkingInput && speedThres);

        // Move player relative to camera, but projected to ground
        Vector3 inputV = Vector3.ClampMagnitude(new Vector3(h, v), 1);
        Transform camT = Camera.main.transform;
        Vector3 camRelativeV = camT.right * inputV.x + camT.up * inputV.y;

        Vector3 targetPosition = transform.position + camRelativeV;
        targetPosition.y = 0;

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = new Ray(camRelativeV, camT.forward);

        float enter;
        groundPlane.Raycast(ray, out enter);
        Vector3 forceV = ray.GetPoint(enter) * speed;

        rb.AddForce(forceV);

        lastPos = transform.position;
    }
}
