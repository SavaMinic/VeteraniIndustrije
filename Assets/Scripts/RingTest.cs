using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Attemping to ring");
            Bell.e.Ring();

        }
    }
}
