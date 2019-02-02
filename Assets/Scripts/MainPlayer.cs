using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public float speed;
    public string HorizontalAxis;
    public string VerticalAxis;

    private Rigidbody rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        float h = Input.GetAxis(HorizontalAxis);
        float v = Input.GetAxis(VerticalAxis);

        if (Mathf.Abs(h) > 0)
        {
            spriteRenderer.flipX = h > 0f;
        }
        animator.SetBool("walking", Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);

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
    }
}
