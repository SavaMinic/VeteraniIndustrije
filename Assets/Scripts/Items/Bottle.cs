using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NDraw;

/*
public interface IDrinkContainer
{
    Drink drinkType { get; set; }
    float amount { get; set; }
}*/

public class Bottle : Item//, IDrinkContainer
{
    public DrinkContainer container;

    Slavina slavina;

    private void Update()
    {
        //if (container.amount > 0)
            //DebugUtils.InGameMeter(container.amount / container.maxAmount, transform.position, 20, DrinkFoodUtils.GetColor(container.drinkType));
    }

    private void OnDrawGizmos()
    {
        if (container)
            DebugUtils.Meter(container.amount / container.maxAmount, transform.position, 0.2f, DrinkFoodUtils.GetColor(container.drinkType));
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Slavina)
        {
            slavina = slot as Slavina;
            slavina.ShowMlaz();
        }
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        if (slot is Slavina)
        {
            slavina.EndPouringWater();
        }
    }
}
