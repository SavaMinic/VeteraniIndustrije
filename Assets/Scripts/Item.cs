﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IProximityFindable
{
    public static List<Interactable> all;

    public bool isTakeable = true;
    public bool isHeld;
    bool PlayerCantInteract { get; set; }
    public bool SkipProximitySearch => PlayerCantInteract;

    Hands inHands;
    public Slot inSlot;

    public Color highlightColor;
    public SpriteRenderer sprite;

    public Transform customProximityPivot;

    public float soundVolume = 0.5f;
    public AudioClip pickupClip;
    public AudioClip putClip;

    public Vector3 ProximityPosition
    {
        get
        {
            if (customProximityPivot)
                return customProximityPivot.position;
            else
                return transform.position;
        }
    }

    protected void Start()
    {
        if (all == null)
            all = new List<Interactable>();
        //all = new List<Item>(FindObectsOfType<Item>());

        all.Add(this);

        if (!sprite)
            sprite = GetComponent<SpriteRenderer>();

        OnStart();
    }

    protected virtual void OnStart() { }
    protected virtual void OnBeforeDestroy() { }

    protected void OnDestroy()
    {
        OnBeforeDestroy();
        all.Remove(this);
    }

    public void EnablePlayerInteraction(bool canPlayerInteract)
    {
        if (!canPlayerInteract)
            Highlight(false);

        PlayerCantInteract = !canPlayerInteract;
    }

    public Interactable Take(Hands hands)
    {
        if (hands.heldItem) return null; // cant take in occupied hands

        if (inSlot)
        {
            OnRemovedFromSlot(inSlot);
            inSlot.OnItemRemoved();
            inSlot.itemInSlot = null;
        }

        inSlot = null;

        transform.SetParent(hands.transform);
        transform.localPosition = hands.holdOffset;
        isHeld = true;
        inHands = hands;

        if (pickupClip) pickupClip.Play2D(soundVolume);

        Debug.Log($"Took {name}");

        return this;
    }

    public bool PlaceInSlot(Slot slot)
    {
        // cant place in occupied slot
        if (slot.itemInSlot)
            return false;
        // cant place if that slot doesn't interact with this item
        if (slot.useInteractionControl && !InteractionControl.I.CanInteract(gameObject, slot.gameObject))
            return false;

        if (slot.onlyInteractsWith && slot.onlyInteractsWith != this)
            return false;

        transform.SetParent(slot.transform);
        transform.localPosition = Vector3.zero; // temp, TODO: Animate
        isHeld = false;
        inSlot = slot;

        slot.itemInSlot = this;

        OnPlacedInSlot(slot);
        slot.OnItemPlaced();

        if (putClip) putClip.Play2D(soundVolume);
        return true;
    }

    public void Remove()
    {
        if (inSlot)
        {
            inSlot.itemInSlot = null;
        }

        if (inHands)
        {
            isHeld = false;
            inHands.heldItem = null;
        }

        Destroy(gameObject);
    }

    protected virtual void OnPlacedInSlot(Slot slot) { }
    protected virtual void OnRemovedFromSlot(Slot slot) { }

    public void Highlight(bool b)
    {
        if (sprite)
        {
            sprite.color = b ? highlightColor : Color.white;
        }

        var childSprites = GetComponentsInChildren<SpriteRenderer>();
        if (childSprites != null && childSprites.Length > 0)
        {
            for (int i = 0; i < childSprites.Length; i++)
            {
                if (childSprites[i].name == "Liquid")
                    continue;

                childSprites[i].color = b ? highlightColor : Color.white;
            }
        }
    }
}
