using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtils
{
    public static void Meter(float value, Vector3 position, float offset, Color color)
    {
        Debug.DrawRay(position + Vector3.right * offset, Vector3.up, Color.gray);
        Debug.DrawRay(position + Vector3.right * offset, Vector3.up * value, color);
    }
}
