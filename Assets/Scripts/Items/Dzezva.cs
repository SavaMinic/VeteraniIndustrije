using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dzezva : Interactable
{
    public DrinkContainer container;
    public bool hasCoffee;

    public float kipi;

    public float heatingSpeed = 0.1f;
    public float kipiSpeed = 0.1f;
    public float coolingSpeed = 0.01f;

    public Ringla ringla;
    public Slavina slavina;


    private void Update()
    {
        if (ringla && ringla.isOn && container.amount >= 1)
        {
            container.temperature += Time.deltaTime * heatingSpeed;

            if (container.temperature >= 1)
                kipi += Time.deltaTime * kipiSpeed;
        }
        else
        {
            container.temperature -= Time.deltaTime * coolingSpeed;
            kipi -= Time.deltaTime * kipiSpeed;
        }

        /*
        if (slavina)
        {
            container.drinkType = Drink.Water;
            container.amount += Time.deltaTime * slavina.fillSpeed;
        }*/

        container.temperature = Mathf.Clamp01(container.temperature);
        kipi = Mathf.Clamp01(kipi);
        container.amount = Mathf.Clamp01(container.amount);

        // Debug lines
        DebugUtils.Meter(container.temperature, transform.position, 0.2f, Color.red);
        DebugUtils.Meter(kipi, transform.position, 0.3f, Color.yellow);
        DebugUtils.Meter(container.amount, transform.position, 0.4f, DrinkFoodUtils.GetColor(container.drinkType));
    }

    protected override void OnPlacedInSlot(Slot slot)
    {
        if (slot is Ringla)
            ringla = slot as Ringla;


        if (slot is Slavina)
        {
            //container.drinkType = Drink.Water;
            slavina = slot as Slavina;
            slavina.ShowMlaz();
        }
    }

    protected override void OnRemovedFromSlot(Slot slot)
    {
        if (slavina)
        {
            slavina.EndPouringWater();
        }

        ringla = null;
        slavina = null;
    }
}
