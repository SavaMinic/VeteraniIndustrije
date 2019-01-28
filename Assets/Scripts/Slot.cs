using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public static Slot[] all;

    public Interactable itemInSlot;

    public bool dontRenderWhenNotHighlighted;
    public Color highlightColor;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Slot>();

        if (dontRenderWhenNotHighlighted)
            Highlight(false);
    }

    public void Highlight(bool b)
    {
        var sprite = GetComponent<SpriteRenderer>();

        if (!sprite) return;

        sprite.color = b ? highlightColor : Color.white;

        if (dontRenderWhenNotHighlighted)
            sprite.enabled = b;
    }
}
