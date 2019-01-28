using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : Interactable
{
    public DrinkContainer container;

    private void Update()
    {
        DebugUtils.Meter(container.amount, transform.position, 0.2f, DrinkFoodUtils.GetColor(container.drinkType));
    }
}
