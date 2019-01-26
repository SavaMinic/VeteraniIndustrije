using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public Item heldItem;

    public Vector3 holdOffset;

    private void Update()
    {
        Slot closestSlot = Slot.FindClosest(transform.position);
        Debug.DrawLine(transform.position, closestSlot.transform.position, Color.red);

        Item closestItem = Item.FindClosest(transform.position);
        Debug.DrawLine(transform.position, closestItem.transform.position, Color.green);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!heldItem)
            {
                heldItem = closestItem.Take(this);
                //if (heldItem)
            }
            else
            {
                if (closestSlot)
                {
                    heldItem.PlaceInSlot(closestSlot);
                    heldItem = null;
                }
            }
        }
    }
}