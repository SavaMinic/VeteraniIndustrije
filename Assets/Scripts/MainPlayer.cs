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
        
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Debug.LogError(moveHorizontal + " " + moveVertical);

        Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

        rb.AddForce (movement * speed);
    }
}
