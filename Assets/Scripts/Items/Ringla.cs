using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ringla : Slot
{
    public bool isOn;

    public AnimationCurve vriVolumePerTemperature;

    public AudioSource vriSource;

    public AudioClip turnOnClip;
    public AudioClip turnOffClip;

    private void Update()
    {
        var dzezva = itemInSlot as Dzezva;

        vriSource.enabled = itemInSlot == true;

        if (dzezva)
        {
            vriSource.volume = vriVolumePerTemperature.Evaluate(dzezva.container.temperature);
        }
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
