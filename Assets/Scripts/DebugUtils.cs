using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NDraw;

public static class DebugUtils
{
    const float scale = 5;

    public static void Meter(float value, Vector3 position, float offset, Color color)
    {
        Debug.DrawRay(position + Vector3.right * offset * scale, Vector3.up * scale, Color.gray);
        Debug.DrawRay(position + Vector3.right * offset * scale, Vector3.up * value * scale, color);
    }

    public static void InGameMeter(float value, Vector3 position, float offset, Color color)
    {
        const float heightMult = 100;

        Draw.Screen.SetFillColor(color);
        Draw.Screen.SetColor(Color.black);
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        Draw.Screen.Rect(pos.x + offset, pos.y, 10, heightMult);
        Draw.Screen.Rect(pos.x + offset - 1, pos.y - 1, 10 + 2, heightMult + 2);
        Draw.Screen.FillRect(pos.x + offset, pos.y, 10, heightMult * value);
    }
}
