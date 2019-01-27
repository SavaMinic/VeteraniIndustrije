using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public static Item[] all;

    public bool isTakeable = true;
    public bool isHeld;

    Hands inHands;
    public Slot inSlot;

    public Color highlightColor;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Item>();
    }

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

    public void Highlight(bool b)
    {
        var sprite = GetComponent<SpriteRenderer>();

        if (sprite)
        {
            sprite.color = b ? highlightColor : Color.white;
        }

        var childSprites = GetComponentsInChildren<SpriteRenderer>();
        if (childSprites != null && childSprites.Length > 0)
        {
            for (int i = 0; i < childSprites.Length; i++)
            {
                childSprites[i].color = b ? highlightColor : Color.white;
            }
        }
    }
}
