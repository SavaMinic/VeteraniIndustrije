using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slavina : Slot
{
    public float fillSpeed = 0.5f;

    public GameObject runningWaterGraphics;

    public void PourWater()
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(true);
    }

    public void EndPouringWater()
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(false);
    }
}
