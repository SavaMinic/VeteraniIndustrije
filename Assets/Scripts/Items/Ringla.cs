using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ringla : Slot
{
    public bool isOn;

    public AnimationCurve shumiVolumePerTemperature;
    public AnimationCurve vriVolumePerTemperature;


    public AudioSource shumiSource;
    public AudioSource vriSource;

    public AudioClip turnOnClip;
    public AudioClip turnOffClip;

    private void Update()
    {
        var dzezva = itemInSlot as Dzezva;

        if (dzezva)
        {
            shumiSource.volume = shumiVolumePerTemperature.Evaluate(dzezva.container.temperature);
            vriSource.volume = vriVolumePerTemperature.Evaluate(dzezva.container.temperature);
        }
    }

    public override void OnItemPlaced()
    {
        base.OnItemPlaced();
        if (itemInSlot && itemInSlot is Dzezva)
        {
            vriSource.enabled = true;
            shumiSource.enabled = true;
            turnOnClip.Play2D();
        }
    }

    public override void OnItemRemoved()
    {
        base.OnItemRemoved();
        if (itemInSlot && itemInSlot is Dzezva)
        {
            turnOffClip.Play2D();
            vriSource.enabled = false;
            shumiSource.enabled = false;
        }

    }
}
