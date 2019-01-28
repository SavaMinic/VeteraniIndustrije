using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slavina : Slot
{
    public float fillSpeed = 0.5f;

    public GameObject runningWaterGraphics;

    private void Update()
    {
        DrinkContainer dc = itemInSlot?.GetComponent<DrinkContainer>();

        if (dc && dc.canBeRefilledAtSlavina)
        {
            dc.drinkType = Drink.Water;
            dc.amount += Time.deltaTime * fillSpeed;
            if (dc.amount > dc.maxAmount) dc.amount = dc.maxAmount;
        }
    }

    public void ShowMlaz(bool b)
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(b);
    }

    public override void OnItemPlaced()
    {
        DrinkContainer dc = itemInSlot?.GetComponent<DrinkContainer>();

        if (dc && dc.canBeRefilledAtSlavina)
            ShowMlaz(true);
    }

    public override void OnItemRemoved()
    {
        ShowMlaz(false);
    }
}
