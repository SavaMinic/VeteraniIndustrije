using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public static T FindClosest<T>(T[] array, Vector3 target, float maxRange = Mathf.Infinity) where T : Component
    {
        if (array == null) return null;

        float closestDistance = Mathf.Infinity;
        T closest = null;

        for (int i = 0; i < array.Length; i++)
        {
            float sqrdist = (target - array[i].transform.position).sqrMagnitude;

            if (sqrdist < closestDistance)
            {
                closestDistance = sqrdist;
                closest = array[i];
            }
        }

        if (closestDistance > maxRange * maxRange)
            return null;

        return closest;
    }
}
