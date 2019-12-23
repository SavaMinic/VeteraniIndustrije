using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rerna : Slot
{
    public float heatingSpeed = 0.1f;

    public SpriteRenderer closedSprite;
    public SpriteRenderer overlaySprite;
    public SpriteRenderer openSprite;

    public AudioClip rernaOpen;
    public AudioClip rernaClose;

    public float openCloseVolume = 0.2f;

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        if (itemInSlot is LonacSarme)
        {
            Container container = itemInSlot.GetComponent<Container>();
            container.AddHeat(Time.deltaTime * heatingSpeed);
        }
    }

    public override void OnItemRemoved()
    {
        closedSprite.enabled = false;
        overlaySprite.enabled = false;

        openSprite.enabled = true;

        rernaOpen?.Play2D(openCloseVolume);
    }

    public override void OnItemPlaced()
    {
        closedSprite.enabled = true;
        overlaySprite.enabled = true;

        openSprite.enabled = false;

        rernaClose?.Play2D(openCloseVolume);
    }
}