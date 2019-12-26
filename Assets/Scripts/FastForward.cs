using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastForward : MonoBehaviour
{
    void Update()
    {
        if (!Application.isEditor && !GuestManager.showDebugWindow)
            return;

        if (Input.GetKey(KeyCode.Alpha1))
            Time.timeScale = 5;
        else if (Input.GetKeyUp(KeyCode.Alpha1))
            Time.timeScale = 1;
    }
}
