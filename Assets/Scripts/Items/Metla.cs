using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metla : Interactable
{
    private SpriteRenderer metlaRenderer;
    private float verticalInSlot;

    
    protected override void OnStart()
    {
        base.OnStart();
        metlaRenderer = GetComponentInChildren<SpriteRenderer>();
        verticalInSlot = metlaRenderer.transform.localPosition.y;
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        base.OnPlacedInSlot(slot);

        var pos = metlaRenderer.transform.localPosition;
        pos.y = verticalInSlot;
        metlaRenderer.transform.localPosition = pos;
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        base.OnRemovedFromSlot(slot);
        
        var pos = metlaRenderer.transform.localPosition;
        pos.y = 0f;
        metlaRenderer.transform.localPosition = pos;
    }
}
