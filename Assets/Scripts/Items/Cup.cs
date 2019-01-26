using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : Item
{
    public Drink drinkType;
    public float amount;

    private void Update()
    {
        DebugUtils.Meter(amount, transform.position, 0.2f, DrinkFoodUtils.GetColor(drinkType));
    }
}
