using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;
    
    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate ()
    {
        if (!Application.isPlaying)
            return;
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // since camera is rotated, recalculate the movement so x/z axis are appropriate for camera
        // minus is there because camera is looking back
        Vector3 movement =  - new Vector3(v + h, 0.0f, v - h);

        rb.AddForce (movement * speed);
    }
}
