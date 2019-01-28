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

    public static T FindClosest<T>(T[] array, T skip, Vector3 target, float maxRange = Mathf.Infinity, bool inViewSpace = false) where T : Component
    {
        if (array == null) return null;

        float closestDistance = Mathf.Infinity;
        T closest = null;

        if (inViewSpace)
        {
            target = GetCameraSpacePoint(target);
        }

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == skip) continue;

            Vector3 pos = array[i].transform.position;
            if (inViewSpace) pos = GetCameraSpacePoint(pos);

            float sqrdist = (target - pos).sqrMagnitude;

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

    public static T FindClosest<T>(List<T> list, T skip, Vector3 target, float maxRange = Mathf.Infinity, bool inViewSpace = false) where T : Component
    {
        if (list == null) return null;

        float closestDistance = Mathf.Infinity;
        T closest = null;

        if (inViewSpace)
        {
            target = GetCameraSpacePoint(target);
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == skip) continue;

            Vector3 pos = list[i].transform.position;
            if (inViewSpace) pos = GetCameraSpacePoint(pos);

            float sqrdist = (target - pos).sqrMagnitude;

            if (sqrdist < closestDistance)
            {
                closestDistance = sqrdist;
                closest = list[i];
            }
        }

        if (closestDistance > maxRange * maxRange)
            return null;

        return closest;
    }

    public static Vector3 GetCameraSpacePoint(Vector3 v3)
    {
        v3 = Camera.main.transform.InverseTransformPoint(v3);
        v3.z = 0;
        return v3;
    }
}
