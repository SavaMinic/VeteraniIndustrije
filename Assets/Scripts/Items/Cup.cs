using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : Item
{
    public Drink drinkType;
    public float amount;

    Slavina slavina;

    private void Update()
    {
        DebugUtils.Meter(amount, transform.position, 0.2f, DrinkFoodUtils.GetColor(drinkType));

        if (slavina)
        {
            amount += Time.deltaTime * slavina.fillSpeed;
        }
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Slavina)
        {
            slavina = slot as Slavina;
            slavina.PourWater();

            if (drinkType != Drink.Water)
            {
                amount = 0;
                drinkType = Drink.Water;
            }
        }
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        if (slavina)
        {
            slavina.EndPouringWater();
            slavina = null;
        }
    }
}
