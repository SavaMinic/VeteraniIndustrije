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
    
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    void FixedUpdate ()
    {
        if (!Application.isPlaying)
            return;
        
        float h = Input.GetAxis(HorizontalAxis);
        float v = Input.GetAxis(VerticalAxis);

        if (Mathf.Abs(h) > 0)
        {
            spriteRenderer.flipX = h > 0f;
        }

        // since camera is rotated, recalculate the movement so x/z axis are appropriate for camera
        // minus is there because camera is looking back
        Vector3 movement =  -new Vector3(v + h, 0.0f, v - h);

        rb.AddForce (movement * speed);
    }
}
