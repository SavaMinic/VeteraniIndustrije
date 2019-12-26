using System;
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

    public static T FindClosest<T>(List<T> list, Vector3 target, float maxRange = Mathf.Infinity, bool inViewSpace = false)
        where T : Component, IProximityFindable
    {
        if (list == null || list.Count == 0) return null;

        float closestDistance = Mathf.Infinity;
        T closest = null;

        if (inViewSpace)
        {
            target = GetCameraSpacePoint(target);
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].SkipProximitySearch) continue;

            Vector3 pos = list[i].ProximityPosition;
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

    public static bool IsCloserThan<T>(this T tis, T other, Vector3 target, bool inViewSpace = false)
        where T : IProximityFindable
    {
        target = inViewSpace ? GetCameraSpacePoint(target) : target;
        Vector3 tisPos = inViewSpace ? GetCameraSpacePoint(tis.ProximityPosition) : tis.ProximityPosition;
        Vector3 otherPos = inViewSpace ? GetCameraSpacePoint(other.ProximityPosition) : other.ProximityPosition;

        float tisDist = (target - tisPos).sqrMagnitude;
        float otherDist = (target - otherPos).sqrMagnitude;

        return tisDist < otherDist;
    }

    [System.Obsolete]
    public static T FindClosest<T>(T[] array, T skip, Vector3 target, float maxRange = Mathf.Infinity, bool inViewSpace = false, Func<T, bool> skipCheck = null)
        where T : Component, IProximityFindable
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
            if (array[i] == skip || array[i].SkipProximitySearch) continue;

            if (array[i] == skip) continue;
            if (skipCheck != null && skipCheck(array[i])) continue;

            Vector3 pos = array[i].ProximityPosition;
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

    [System.Obsolete]
    public static T FindClosest<T>(List<T> list, T skip, Vector3 target, float maxRange = Mathf.Infinity, bool inViewSpace = false)
        where T : Component, IProximityFindable
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
            if (list[i] == skip || list[i].SkipProximitySearch) continue;

            Vector3 pos = list[i].ProximityPosition;
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

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
