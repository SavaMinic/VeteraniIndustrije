using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : Interactable
{
    public DrinkContainer container;

    Slavina slavina;

    private void Update()
    {
        DebugUtils.Meter(container.amount, transform.position, 0.2f, DrinkFoodUtils.GetColor(container.drinkType));

        /*
        if (slavina)
        {
            container.amount += Time.deltaTime * slavina.fillSpeed;
        }*/
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Slavina)
        {
            slavina = slot as Slavina;
            slavina.ShowMlaz();

            /*
            if (container.drinkType != Drink.Water)
            {
                container.amount = 0;
                container.drinkType = Drink.Water;
            }*/
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
