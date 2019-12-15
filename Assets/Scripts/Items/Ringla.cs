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

    float shumiVolumeVelo;
    float vriVolumeVelo;

    const float audioSmoothing = 0.5f;

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        var dzezva = itemInSlot as Dzezva;

        float shumiTargetVolume = 0;
        float vriTargetVolume = 0;

        if (dzezva)
        {
            shumiTargetVolume = shumiVolumePerTemperature.Evaluate(dzezva.container.temperature);
            vriTargetVolume = vriVolumePerTemperature.Evaluate(dzezva.container.temperature);
        }

        shumiSource.volume = Mathf.SmoothDamp(shumiSource.volume, shumiTargetVolume, ref shumiVolumeVelo, audioSmoothing);
        vriSource.volume = Mathf.SmoothDamp(vriSource.volume, vriTargetVolume, ref vriVolumeVelo, audioSmoothing);
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
            //vriSource.enabled = false;
            //shumiSource.enabled = false;
        }

    }
}
