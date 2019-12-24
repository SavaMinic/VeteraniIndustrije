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

    public Transform flekaFloorPoint;

    float shumiVolumeVelo;
    float vriVolumeVelo;

    const float audioSmoothing = 0.5f;

    float lastFlekaTime = 0;
    bool wasBrbotjing;

    private void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        var dzezva = itemInSlot as Dzezva;

        float shumiTargetVolume = 0;
        float vriTargetVolume = 0;

        if (dzezva)
        {
            if (dzezva.container.amount > 0)
            {
                shumiTargetVolume = shumiVolumePerTemperature.Evaluate(dzezva.container.temperature);
                vriTargetVolume = vriVolumePerTemperature.Evaluate(dzezva.container.temperature);

                if (dzezva.container.type.name == "Kafa"
                    && dzezva.container.amount > 0.7f
                    && dzezva.container.temperature >= 1)
                {
                    if (!wasBrbotjing)
                    {
                        lastFlekaTime = Time.time + 3;
                        wasBrbotjing = true;
                    }
                    else if (Time.time - lastFlekaTime > 0)
                    {

                        Vector3 r = Random.insideUnitSphere;
                        r.y = 0;
                        Database.e.CreateFleka(flekaFloorPoint.position + r);

                        lastFlekaTime = Time.time + Random.Range(3.0f, 5.0f);
                    }
                }
                else
                    wasBrbotjing = false;
            }
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
