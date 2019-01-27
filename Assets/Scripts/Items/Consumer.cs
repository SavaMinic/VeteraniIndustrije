using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : Item
{
    public FoodContainer foodContainer;
    public DrinkContainer drinkContainer;

    protected override void OnPlacedInSlot(Slot slot)
    {
    }

    private void Update()
    {
        foodContainer.DebugAmount(0.2f);
        drinkContainer.DebugAmount(0.4f);
    }
}
