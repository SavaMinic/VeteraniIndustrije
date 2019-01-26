using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Cache and closest item
    public static Item[] allItems;

    public bool isHeld;

    Hands inHands;
    Slot inSlot;

    private void Start()
    {
        if (allItems == null)
            allItems = FindObjectsOfType<Item>();
    }

    public static Item FindClosest(Vector3 target)
    {
        if (allItems == null) return null;

        float closestDistance = Mathf.Infinity;
        Item closest = null;

        for (int i = 0; i < allItems.Length; i++)
        {
            float sqrdist = (target - allItems[i].transform.position).sqrMagnitude;
            if (sqrdist < closestDistance)
            {
                closestDistance = sqrdist;
                closest = allItems[i];
            }
        }

        return closest;
    }
    #endregion

    public Item Take(Hands hands)
    {
        if (hands.heldItem) return null; // cant take in occupied hands

        if (inSlot) OnRemovedFromSlot(inSlot);

        inSlot = null;

        transform.SetParent(hands.transform);
        transform.localPosition = hands.holdOffset;
        isHeld = true;
        inHands = hands;

        return this;
    }

    public void PlaceInSlot(Slot slot)
    {
        if (slot.itemInSlot) return; // cant place in occupied slot

        transform.SetParent(slot.transform);
        transform.localPosition = Vector3.zero; // temp, TODO: Animate
        isHeld = false;
        inSlot = slot;

        OnPlacedInSlot(slot);
    }

    protected virtual void OnPlacedInSlot(Slot slot) { }
    protected virtual void OnRemovedFromSlot(Slot slot) { }
}
