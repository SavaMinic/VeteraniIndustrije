using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ringla : Slot
{
    public bool isOn;

    public AudioSource vriSource;

    public AudioClip turnOnClip;
    public AudioClip turnOffClip;

    private void Update()
    {
        vriSource.enabled = itemInSlot == true;
    }

    public override void OnItemPlaced()
    {
        if (itemInSlot && itemInSlot is Dzezva)
            turnOnClip.Play2D();
    }

    public override void OnItemRemoved()
    {
        if (itemInSlot && itemInSlot is Dzezva)
            turnOffClip.Play2D();
    }
}
