using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slavina : Slot
{
    public float fillSpeed = 0.5f;
    public float coolingSpeed = 0.2f;

    public GameObject runningWaterGraphics;

    private void Update()
    {
        DrinkContainer dc = itemInSlot?.GetComponent<DrinkContainer>();

        if (dc && dc.canBeRefilledAtSlavina)
        {
            dc.AddDrink(Drink.Water, Time.deltaTime * fillSpeed);
            dc.AddHeat(-coolingSpeed * Time.deltaTime);
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
