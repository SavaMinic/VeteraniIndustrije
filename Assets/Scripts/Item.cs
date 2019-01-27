using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Cache and closest item
    public static Item[] all;

    public bool isTakeable = true;
    public bool isHeld;

    Hands inHands;
    public Slot inSlot;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Item>();
    }
    #endregion

    public Item Take(Hands hands)
    {
        if (hands.heldItem) return null; // cant take in occupied hands

        if (inSlot)
        {
            inSlot.itemInSlot = null;
            OnRemovedFromSlot(inSlot);
        }

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

        slot.itemInSlot = this;

        OnPlacedInSlot(slot);
    }

    protected virtual void OnPlacedInSlot(Slot slot) { }
    protected virtual void OnRemovedFromSlot(Slot slot) { }
}
