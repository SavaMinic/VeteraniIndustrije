using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtils
{
    const float scale = 5;

    public static void Meter(float value, Vector3 position, float offset, Color color)
    {
        Debug.DrawRay(position + Vector3.right * offset * scale, Vector3.up * scale, Color.gray);
        Debug.DrawRay(position + Vector3.right * offset * scale, Vector3.up * value * scale, color);
    }
}
