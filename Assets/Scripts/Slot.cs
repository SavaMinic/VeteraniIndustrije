using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public static Slot[] all;

    public Item itemInSlot;

    public Color highlightColor;

    private void Start()
    {
        if (all == null)
            all = FindObjectsOfType<Slot>();
    }

    public void Highlight(bool b)
    {
        var sprite = GetComponent<SpriteRenderer>();

        if (!sprite) return;

        sprite.color = b ? highlightColor : Color.white;
    }
}
