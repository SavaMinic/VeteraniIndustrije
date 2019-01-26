using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public static Slot[] allSlots;

    public Item itemInSlot;

    private void Start()
    {
        if (allSlots == null)
            allSlots = FindObjectsOfType<Slot>();
    }

    public static Slot FindClosest(Vector3 target)
    {
        if (allSlots == null) return null;

        float closestDistance = Mathf.Infinity;
        Slot closest = null;

        for (int i = 0; i < allSlots.Length; i++)
        {
            float sqrdist = (target - allSlots[i].transform.position).sqrMagnitude;
            if (sqrdist < closestDistance)
            {
                closestDistance = sqrdist;
                closest = allSlots[i];
            }
        }

        return closest;
    }
}
