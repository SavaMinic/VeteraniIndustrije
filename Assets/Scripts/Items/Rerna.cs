using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rerna : Slot
{
    public float heatingSpeed = 0.1f;

    private void Update()
    {
        if (itemInSlot is LonacSarme)
        {
            FoodContainer container = itemInSlot.GetComponent<FoodContainer>();
            container.temperature += Time.deltaTime * heatingSpeed;
            container.temperature = Mathf.Clamp01(container.temperature);
        }
    }
}
