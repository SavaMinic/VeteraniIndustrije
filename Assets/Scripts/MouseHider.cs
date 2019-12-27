using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHider : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.visible = false;
        }
    }
}
