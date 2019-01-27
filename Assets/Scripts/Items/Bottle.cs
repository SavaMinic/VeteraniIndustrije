using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public interface IDrinkContainer
{
    Drink drinkType { get; set; }
    float amount { get; set; }
}*/

public class Bottle : Item//, IDrinkContainer
{
    public DrinkContainer container;

    private void OnDrawGizmos()
    {
        if (container)
            DebugUtils.Meter(container.amount / container.maxAmount, transform.position, 0.2f, DrinkFoodUtils.GetColor(container.drinkType));
    }
}
