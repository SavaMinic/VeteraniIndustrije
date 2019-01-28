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

public class Bottle : Interactable//, IDrinkContainer
{
    public DrinkContainer container;
    
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
}
