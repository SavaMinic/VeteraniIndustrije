using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rerna : Slot
{
    public float heatingSpeed = 0.1f;

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
}