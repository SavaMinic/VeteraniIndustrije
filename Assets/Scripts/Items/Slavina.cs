using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slavina : Slot
{
    public Consumable liquidType;

    public float fillSpeed = 0.5f;
    public float coolingSpeed = 0.2f;

    public GameObject runningWaterGraphics;

    private void Update()
    {
        Container c = itemInSlot?.GetComponent<Container>();

        if (c && c.canBeRefilledAtSlavina)
        {
            c.AddDrink(liquidType, Time.deltaTime * fillSpeed);
            c.AddHeat(-coolingSpeed * Time.deltaTime);
        }
    }

    public void ShowMlaz(bool b)
    {
        if (runningWaterGraphics)
            runningWaterGraphics.SetActive(b);
    }

    public override void OnItemPlaced()
    {
        base.OnItemPlaced();
        Container dc = itemInSlot?.GetComponent<Container>();

        if (dc && dc.canBeRefilledAtSlavina)
            ShowMlaz(true);
    }

    public override void OnItemRemoved()
    {
        base.OnItemRemoved();
        ShowMlaz(false);
    }
}
