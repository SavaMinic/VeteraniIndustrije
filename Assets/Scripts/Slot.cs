using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour, IProximityFindable
{
    public static Slot[] all;

    // if this is set, only this item can be put in slot
    public Interactable onlyInteractsWith;

    public Interactable itemInSlot;

    public bool useInteractionControl;

    public bool dontRenderWhenNotHighlighted;
    public Color highlightColor;

    public bool SkipProximitySearch => false;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Slot>();

        if (dontRenderWhenNotHighlighted)
            Highlight(false);
    }

    private void OnDestroy()
    {
        if (all != null)
        {
            all = null;
        }
    }

    public void Highlight(bool b)
    {
        var sprite = GetComponent<SpriteRenderer>();

        if (!sprite) return;

        sprite.color = b ? highlightColor : Color.white;

        if (dontRenderWhenNotHighlighted)
            sprite.enabled = b;
    }

    public virtual void OnItemPlaced() { }
    public virtual void OnItemRemoved() { }
}
