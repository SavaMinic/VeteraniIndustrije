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
    public Drink drinkType;
    public float amount;
    public float maxAmount = 10;

    private void OnDrawGizmos()
    {
        DebugUtils.Meter(amount / maxAmount, transform.position, 0.2f, DrinkFoodUtils.GetColor(drinkType));
    }
}
